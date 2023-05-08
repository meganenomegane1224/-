using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//主人公の移動、HP等を制御するスクリプト
public class Move : MonoBehaviour
{
	private CharacterController characterController;//主人公用の当たり判定をアタッチ
	[HideInInspector]
	public Vector3 velocity;                        //移動方向を定義
	private Animator animator;                      //animatorをアタッチ
	private float Speed;                            //animatorに渡すspeed量を定義
    public MyState state;                           //キャラの状態を定義
	private Quaternion targetRotation;              //キャラクターが向く方向を定義
	private Admin admin;                            //adminをアタッチ
	private AudioSource audioSource;                //audiosouceをアタッチ
	[SerializeField]
	private AudioClip[] sound;                      //主人公が使う効果音を格納
	[SerializeField]
	private Admin_Effect[] effects;                 //主人公が使うエフェクト(prefabではなく、あらかじめインスタンス化すること)を格納
	// HP関連

    public float charahp;                          //現在HPを定義
    [SerializeField]
    public Slider slider;	                        // シーンに配置したSliderを格納
	[SerializeField]
	private float lockInterval;                     //再ロックができるようになる時間を定義  
	[SerializeField]
	private float physicalStrength = 15;            //回避等で使うスタミナ量を定義
	private Slider physicalStrengthSlider;          //スタミナをバー上に表示するsliderを定義
	[SerializeField]
	private Canvas staminaCanvas;                   //スタミナを表示させるcanvasを格納
	[SerializeField]
	private CanvasGroup AvoidanceDisplayEffect;     //回避時に出す画面エフェクトのcanvsGroupを格納
	private int animSpeedHash;                      //移動に使うfloat値のアッシュと定義
	public bool AutoRunFlag = true;                 //自動ダッシュをするかしないか定義
	[SerializeField]
	private Camera mainCamera;                      //mainCameraをアタッチ　windowaで操作するとき利用
	[SerializeField]
	private GameObject ParentCamera;                //カメラの起点となる親オブジェクトをアタッチ mobileで操作するとき利用
	[SerializeField]
	private FixedJoystick fixedJoystick;            //モバイルで操作する時使用する仮想コントローラーをアタッチ
	public bool isWorkeingMobilePlatform;           //モバイルで動いているかいないか定義
	[SerializeField]
	private Vector3 ResetPosition;                  //位置異常の時に移動するpositionを設定
	[SerializeField]
	private Admin_UI admin_UI;                      //Admin_UIをアタッチ



	//スクリプト定数
	const float JUST_AVOIDANCE_TIME_SCALE = 0.2f;
	const float LOCK_INTERVAL = 3;
	const float MAX_PHYSICAL_STRENGH = 15;
	const float CHARACTER_ROTATE_SPEED = 600;
	const float EFFECT_VOLUME = 0.4f;
	const float REVIVE_TIME = 8;

	void Awake() 
    {
		//スタートするときに、前回保存した位置に移動する
		var posi = new Vector3(PlayerPrefs.GetFloat("posi_X"),PlayerPrefs.GetFloat("posi_y"),PlayerPrefs.GetFloat("posi_z"));
		//ただし、そのY座標がが-1より小さい場合は一以上とみなし警告文を表示し、位置をリセットする
		if(posi.y < -1)
		{
			transform.position = ResetPosition;//位置をリセット
			admin_UI.Alarm(0);//警告文を表示
		}
		else
		{
			transform.position = posi;//保存されたポジションにキャラを移動
		}
		
		targetRotation = transform.rotation;//向く方向を初期化
		//このゲームがモバイルで動いているかを観測し、boolを変更する
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			isWorkeingMobilePlatform = true;
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer) 
		{
			isWorkeingMobilePlatform = false;
		}
	}

	//キャラの状態を定義
	public enum MyState
   {
	Normal,
	Damage,
	Attack,
	SPAttack,
	JUAttack,
	Jump,
	TalkEvent,
	Avoidance,
	Die
    }

	void Start()
	{
		//各コンポーネントをアタッチ
		characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		admin = GetComponentInChildren<Admin>();
		audioSource = GetComponent<AudioSource>();
		physicalStrengthSlider = staminaCanvas.GetComponentInChildren<Slider>();
		//移動に使う2Speedのハッシュを取得
		animSpeedHash = Animator.StringToHash("2Speed");
        charahp =  Admin.HPStatus;      // 初期状態はHP満タン
		slider.maxValue = 1;
        slider.value = 1;	// Sliderの初期状態を設定（HP満タン）
		AvoidanceDisplayEffect.alpha = 0;//回避時の画面エフェクトの透明度を0にする
		physicalStrengthSlider.value = 1;//スタミナの表示量をMAXにする
	}

    public void TakeDamage(float damage)//ダメージを受けたとき、呼ばれる関数
    {
		if(state == MyState.Die)return;//死んでいるなら実行しない

		if(state != MyState.Avoidance && state != MyState.JUAttack)//回避、特殊攻撃でなければダメージを受ける
		{
			SetState(MyState.Damage);//ダメージ状態に変更
			var b = damage - Admin.LastDefenceStatus();
			if(b < 0)b = 0;
			charahp -= b;//現在HPを計算
            slider.value = charahp/ Admin.HPStatus;	// Sliderに現在HPを適用
		}
		else if(state != MyState.JUAttack)//回避中にダメージ判定を受けたら実行
		{
			Time.timeScale = JUST_AVOIDANCE_TIME_SCALE;//時間の流れを遅くする
			StartCoroutine(ResetTimeScale());//時間の流れを直すコルーチンをスタート
			effects[3].gameObject.SetActive(true);//回避エフェクトを表示
			effects[3].EffectStart(0);//エフェクトを再生
			SetState(MyState.JUAttack);//特殊攻撃を発動
			audioSource.volume = EFFECT_VOLUME;
			audioSource.PlayOneShot(sound[11]);//ジャスト回避音を再生
			physicalStrength += 3;//スタミナを回復
			physicalStrengthSlider.value = physicalStrength/MAX_PHYSICAL_STRENGH;//表示するスタミナ量を変更する
		}

		if (slider.value <= 0)//HPが0以下なら死亡させる
        {
			SetState(MyState.Die);
        }
		Debug.Log(charahp);
	}
	

	IEnumerator ResetTimeScale()
	{
		float n = 0;
		while(n<1)//nが1を超えるまでジャスト回避エフェクトの透明度を上げていく
		{
			n = n + 0.3f;
			AvoidanceDisplayEffect.alpha = n;
			yield return new WaitForSecondsRealtime(0.01f);//瞬間的に上がらないように透明度が上がるごとに少し待つ
		}
		yield return  new WaitForSecondsRealtime(0.05f);//タイムスケールをもとに戻すまで待つ
		Time.timeScale = 1;//タイムスケールをもとに戻す
		yield return new WaitForSecondsRealtime(0.15f);//ちょっと待つ
		while(n > 0)//nが0より小さくなるまで画面エフェクトの透明度を下げる；
		{
			n = n - 0.1f;
			AvoidanceDisplayEffect.alpha = n;
			yield return new WaitForSecondsRealtime(0.02f);
		}
		yield return new WaitForSecondsRealtime(1);
		effects[3].EffectEnd();//ジャスト回避エフェクトを終了する
		effects[3].gameObject.SetActive(false);//ジャスト回避エフェクトを非表示にする
	}

	public void LevelUP()
    {
        charahp =  Admin.HPStatus;      // 初期状態はHP満タン
        slider.value = 1;	// Sliderの初期状態を設定（HP満タン）
    }

	// Update is called once per frame
	void Update()
	{
		if(state == MyState.Die)return;//死んでるなら実行しない

		if(Input.GetKeyDown(KeyCode.H))SetState(MyState.JUAttack);

		//スタミナが最大値より低いなら、1秒に1づつ回復させる
		if(physicalStrength < MAX_PHYSICAL_STRENGH)
		{
			physicalStrength += Time.deltaTime;
			physicalStrengthSlider.value = physicalStrength/MAX_PHYSICAL_STRENGH;
		}
		else//スタミナが1以上あるならスタミナを非表示にする
		{
			staminaCanvas.enabled  = false;
		}

		if (state == MyState.Normal)
		{
			//移動方向を定義
			var horizontal = 0f;
			var vertical = 0f;
			//wasdにより入力量を変更
			if(isWorkeingMobilePlatform == false)//パソコンの場合はwasdによって移動量を変更
			{
				horizontal = Input.GetAxis("Horizontal");
				vertical = Input.GetAxis("Vertical");
			}
			else//スマホの場合はバーチャルコントローラーによって移動量を変更
			{
				horizontal = fixedJoystick.Horizontal;
				vertical = fixedJoystick.Vertical;
			}
			
			var t = isWorkeingMobilePlatform? ParentCamera.transform : mainCamera.transform;//カメラの回転量をモバイルであるかないかで参照元を変更
			var horizontalRotation = Quaternion.AngleAxis( t.eulerAngles.y, Vector3.up);//カメラの向きを取得
			velocity = horizontalRotation * new Vector3(horizontal, 0, vertical).normalized;//カメラの向きと入力量から移動方向を決定

			if (velocity.magnitude > 0.5f)
			{
				//回転方向をQuaternionに変換
				targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
			}

			if(AutoRunFlag == false)//自動ダッシュがオフの場合、Shiftキーで走る
			{
				if(Input.GetKey(KeyCode.LeftShift))
				{
					Speed = velocity.magnitude + velocity.magnitude;//スピードを更新
				}
				else
				{
					Speed = velocity.magnitude;//スピードを更新
				}
			}
			else//自動ダッシュがオンの場合は、shiftキーで歩く
			{
				if(Input.GetKey(KeyCode.LeftShift))
				{
					Speed = velocity.magnitude;
				}
				else
				{
					Speed = velocity.magnitude + velocity.magnitude;
				}
			}
			
			// 移動速度をAnimatorに反映
			animator.SetFloat(animSpeedHash, Speed , 0.3f , Time.deltaTime);
			animator.SetBool("Idle", true);
		}
		

		if((Admin.WeaponNumber != 0) &&!EventSystem.current.IsPointerOverGameObject())
		{
			if(isWorkeingMobilePlatform == false && Input.GetButtonDown("Fire1"))GetInput(1);//パソコンで動いているなら、左クリックで攻撃

			if(isWorkeingMobilePlatform == false  && Input.GetMouseButtonDown(1))GetInput(0);//パソコンで動いているなら、右クリックで回避
		}

		
		//会話状態の時は会話相手を向くようにする
		if(state == MyState.TalkEvent)
		{
			Vector3 lockenemy = -transform.position +new Vector3(admin.targetTransform.position.x ,transform.position.y , admin.targetTransform.position.z);
			targetRotation = Quaternion.LookRotation(lockenemy);
		}

		//ロック状態で攻撃状態の時、ロックしている敵を向くようにする
		if(admin.LockOn == true)
		{
			if(state == MyState.Attack || state == MyState.SPAttack || state == MyState.JUAttack)
			{
				Vector3 lockenemy = new Vector3(admin.LockEnemy.transform.position.x ,transform.position.y , admin.LockEnemy.transform.position.z) - transform.position;
				targetRotation = Quaternion.LookRotation(lockenemy);
			}
			//ロック状態でqキーを押すと、ロック状態を解除する
			if(Input.GetKeyDown("q"))admin.LockOff();
		}
		transform.rotation = Quaternion.RotateTowards(transform.rotation , targetRotation , CHARACTER_ROTATE_SPEED * Time.deltaTime);

		//ロック状態の時、再ロックのインターバルをカウント
		if(admin.LockEnemy == true)
		{
			lockInterval += Time.deltaTime;
		}
	}

	//ボタンを押されたときに、またはモバイルで指定のボタンが押されたときに呼ばれる
	public void GetInput(int n)
	{
		if(state == MyState.TalkEvent && state == MyState.JUAttack)return;//会話、特殊攻撃状態なら無効

		//スタミナがあり、回避、ダメージ状態ではない時、回避する
		if(n == 0 && physicalStrength > 2f && state != MyState.Damage && state !=MyState.Avoidance)
		{
			SetState(MyState.Avoidance);
			physicalStrength -= 3;//スタミナを3消費
			physicalStrengthSlider.value = physicalStrength/MAX_PHYSICAL_STRENGH;//スタミナの最大値に対する割合を算出し、スタミナバーに表示
			staminaCanvas.enabled = true;//スタミナを表示する
		}

		//敵を攻撃する
		if(n == 1 && state != MyState.Avoidance)SetState(MyState.Attack);
	}
	
	//キャラの状態を変更する中枢関数
	public void SetState(MyState tempState)
	{
		//Normal状態にする
		if (tempState == MyState.Normal)
		{
			state = MyState.Normal;
		}
		// Attack状態にする
		else if (tempState == MyState.Attack)
		{
			state = MyState.Attack;
			animator.SetTrigger(Admin.WeaponNumber + "Attack");//武器ナンバーに応じた攻撃アニメーションを再生
			if(admin.LockOn == false)//ロック状態ではないなら、ロック可能か判別する関数を起動
			{
				admin.LockOnEnemy();
			}
			else if(lockInterval > LOCK_INTERVAL)//ロック中でロックインターバルが終わっているとき、再ロックでロックを変更するか決める関数を起動
			{
				admin.SecondLockOn();
				lockInterval = 0;
			}
		}
		// 特殊攻撃状態にする
		else if (tempState == MyState.SPAttack)
		{
			state = MyState.SPAttack;
			animator.SetTrigger(Admin.WeaponNumber + "SPAttack");//武器に応じた特殊攻撃のアニメーションを再生する
			if(admin.LockOn == false)//ロック状態ではないなら、ロック可能か判別する関数を起動
			{
				admin.LockOnEnemy();
			}	  
			else if(lockInterval > LOCK_INTERVAL)//ロック中でロックインターバルが終わっているとき、再ロックでロックを変更するか決める関数を起動
			{
				admin.SecondLockOn();
				lockInterval = 0;
			}
		}
		// ジャスト回避状態にする
		else if(tempState == MyState.JUAttack)
		{
			state = MyState.JUAttack;
			if(admin.LockOn == false)
			{
				admin.LockOnEnemy();//ロック状態ではないなら、ロック可能か判別する関数を起動
			}
			animator.SetTrigger(Admin.WeaponNumber + "JUAttack");//武器の応じたジャスト回避攻撃を再生
		}
		else if(tempState == MyState.Jump)//ジャンプ？知らない子ですね
		{
			animator.SetTrigger("Jump");
		}
		// 回避状態にする
		else if (tempState == MyState.Avoidance)
		{
			state = MyState.Avoidance;
			if(Speed > 0.6)//前方に進んでいるなら前方回避、後方に進んでいるなら後方回避を再生
			{
				animator.Play("回避.前方回避1" , 0,0.1f);
			}
			else
			{
				animator.Play("回避.後方回避1" , 0,0.35f);
			}
			ResetEffect();//すべてのエフェクトの再生を中止
		}
		// ダメージ状態にする
		else if(tempState == MyState.Damage)
		{
			state = MyState.Damage;
			velocity = Vector3.zero;//移動を中止
		    animator.Play("Damage");//ダメージアニメーションを再生
		    animator.ResetTrigger(Admin.WeaponNumber + "Attack");//攻撃を中止
			ResetEffect();
		}//すべてのエフェクトを中止
		else if(tempState == MyState.TalkEvent)//会話状態にする
		{
			state = MyState.TalkEvent;
		}
		// 死亡状態にする
		else if(tempState == MyState.Die)
		{
			state = MyState.Die;
			animator.SetTrigger("Die");//死亡アニメーションを再生する
			this.tag = "Untagged";//自身のタグをUntaggedに変更する
			StartCoroutine("Revive");//復活処理のコルーチンを起動
		}
	}
	
	// 敵を復活させるコルーチン関数
	IEnumerator Revive()
	{
		yield return new WaitForSecondsRealtime(REVIVE_TIME);//REVIVE‗TIMEだけまつ
		transform.position = ResetPosition;//位置情報を設定したリセットポジションに変更する
		charahp = Admin.HPStatus;//HPを全回復させる
		slider.value = 1;	//HPバーに変更を適用する
		animator.Play("Idle");//アニメーションのIDLEを再生する
		SetState(MyState.Normal);//状態をノーマルにする
		this.tag = "Player";//タグをプレイヤーに治す
	}

	// 以下　processcharaAnimaEvent
	//アニメーションより命令を受け取り、音楽を再生する
	void AttackStart(int n)
	{
		audioSource.volume = EFFECT_VOLUME;
		audioSource.PlayOneShot(sound[n]);
	}

	// アニメーションより命令を受け取り、足音を再生する
	void FootSound(int n)
	{
		if(n == 0)
		{
			audioSource.volume = 0.4f;
			audioSource.PlayOneShot(sound[Random.Range(12 , 16)]);
		}
	}

	// アニメーションより命令を受け取り、攻撃boolを削除する
	public void LastAttack()
	{
		animator.ResetTrigger(Admin.WeaponNumber + "Attack");
        animator.ResetTrigger(Admin.WeaponNumber + "SPAttack");
	}

	// アニメーションより命令を受け取り、ジャスト回避攻撃、死亡状態ではなけれな状態をノーマルにする
	public void StateEnd()
	{
		if(state == MyState.JUAttack || state == MyState.Die)return;
		SetState(MyState.Normal);
	}

	// ジャスト回避攻撃の時、アニメーションより命令を受け取り、状態をノーマルにする
	void JUAttackEnd()
	{
		SetState(MyState.Normal);
	}

	// アニメーションより命令を受け取り、エフェクトを再生する。旧プログラム　最新のものはEffectOnを使用するとこ
	void Effect(AnimationEvent animationEvent)
	{
		GameObject EffectObj = (Instantiate(animationEvent.objectReferenceParameter,transform.position,transform.rotation,this.transform)as GameObject);
		EffectObj.GetComponent<AttackByEffect>().DamageMagnification = animationEvent.floatParameter;
	}

	// アニメーションより命令を受け取り、エフェクトを出現させる。intはエフェクトの種類、floatはエフェクトごとのtransform情報を選択できる
	void EffectOn(AnimationEvent animationEvent)
	{
		effects[animationEvent.intParameter].gameObject.SetActive(true);
		effects[animationEvent.intParameter].EffectStart((int)animationEvent.floatParameter);
	}

	// アニメーションより命令を受け取り、エフェクトを終了する
	void EffectOff(int n)
	{
		effects[n].EffectEnd();
		effects[n].gameObject.SetActive(false);
	}

	// すべてのエフェクトを終了する
	public void ResetEffect()
	{
		foreach (var item in effects) 
		{
			if(item.gameObject.activeSelf == false)return;
			item.gameObject.SetActive(false);
			item.EffectEnd();
		}
	}
}

