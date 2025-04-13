using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
using Script.Utilities;

public class Player : MonoBehaviour,ISaveable //控制玩家基本操作的类
{
    private Rigidbody2D _rb;
    public float speed;
    private float InputX;
    private float InputY;
    private Vector2 MovementInput;
    private Animator[] animators;
    private bool isMoving;
    private bool inputOff;//禁止玩家输入
    //动画使用工具
    private float mouseX;
    private float mouseY;
    private bool useTool;

    public string GUID => GetComponent<DataGUID>().guid;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        inputOff= true;
    }
    private void Start()
    {
        ISaveable saveable = this;//因为Player类继承了ISaveable接口所有能够直接赋值进ISaveable接口类型
        saveable.RegisterSaveable();//注册Player类放入SaveLoadManager列表中代表是要被保存为数据的类
    }
    private void OnEnable()
    {
        EventSystem.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventSystem.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventSystem.MoveToPosition += OnMoveToPosition;
        EventSystem.MouseClickedEvent += OnMouseClickedEvent;
        EventSystem.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventSystem.StartNewGameEvent += OnStartNewGameEvent;
        EventSystem.EndGameEvent += OnEndGameEvent;
    }
    private void OnDisable()
    {
        EventSystem.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventSystem.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventSystem.MoveToPosition -= OnMoveToPosition;
        EventSystem.MouseClickedEvent -= OnMouseClickedEvent;
        EventSystem.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventSystem.StartNewGameEvent -= OnStartNewGameEvent;
        EventSystem.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        inputOff = true;
    }

    private void OnStartNewGameEvent(int index)
    {
        inputOff = false;
        transform.position = Prams.playerStartPos;
    }

    private void Update()
    {
        if (!inputOff)
        {
            PlayerInput();
        }
        else
        {
            isMoving = false;
        }

        SwitchAnimator();
    }
    private void FixedUpdate()
    {
        if (!inputOff)
            Movement();
    }
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Pause:
                inputOff = true;//玩家不能控制
                break;
            case GameState.Gameplay:
                inputOff = false;//玩家可以控制
                break;
        }
    }

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDatils)
    {
        if (useTool)
            return;
        //TODO:执行动画
        if(itemDatils.itemType!= ItemType.Seed && itemDatils.itemType != ItemType.Commodity && itemDatils.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y+0.85f);
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDatils));
        }
        else
        {
            EventSystem.CallExecuteActionAfterAnimation(mouseWorldPos, itemDatils);
        }
    }
    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos,ItemDetails itemDetails)
    {
        useTool = true;
        inputOff = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("useTool");
            //人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventSystem.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        //等待动画结束
        useTool = false;
        inputOff = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputOff = true;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputOff = false;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
    private void PlayerInput()
    {
        InputX = Input.GetAxisRaw("Horizontal");
        InputY = Input.GetAxisRaw("Vertical");
        if (InputX != 0 && InputY != 0)
        {
            InputX *= 0.6f;
            InputY *= 0.6f;
        }
        //走路状态速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            InputX *= 0.5f;
            InputY *= 0.5f;
        }
        MovementInput = new Vector2(InputX, InputY);
        isMoving = MovementInput != Vector2.zero;
    }
    private void Movement()
    {
        _rb.MovePosition(_rb.position + MovementInput * speed * Time.deltaTime);
    }
    private void SwitchAnimator()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", InputX);
                anim.SetFloat("InputY", InputY);
            }
        }
    }

    public GameSaveData GenerateSaveData()
    {
        //写好了将当前人物坐标数据存入到一个新的GameSaveData
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));
        return saveData;
    }

    public void RestoreData(GameSaveData saveDate)
    {
        var targetPosition = saveDate.characterPosDict[this.name].ToVector3();
        transform.position = targetPosition;
    }
}
