using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

//
// ezDebugger
// @author harry
// @website: http://www.the3frames.com
//
public class ezDebugger : MonoBehaviour {
	private string mWindowTilteName = "ezDebugger";
	private string mDebuggerVersion = "1.0";
	private int intWindowWidth = 1000;
	private int intWindowHeight = 600;
	private static Rect mWindowSizeRect;

	// size
	private int intContentBoardLeft = 5;
	private int intContentBoardTop = 5;
    private int intScrollBoardMarginLeftRight = 20;// gap
	private int intComponentHorizontalGap = 5;
	private int intComponentVerticalGap = 5;
	public int intMinInputAreaHeight = 0;

    public int intScrollWindowHeight = 0;

	// debug textarea
	private Rect mRectDebugTextArea;
	private static string mDebugMessageContent = "";// content
	private static StringBuilder mDebugMessageBuilder = new StringBuilder();

	// debug input textfiled
	private static string mInputMessageContent = "";// content
	private Rect mRectInputTextField;
	private Rect mRectDebugInputSize;
	private Rect mRectEnterBtn;
	public int intDebugInputHeight = 25;

	private static ezDebugger mInstance = null;
	private static bool mIsInitialized = false;
	public static bool IsInitialized
	{
		get{
			return mIsInitialized;
		}
	}

	public static void Init(GameObject go)
	{
		if( go == null )
			throw new ArgumentException("The bundle target must be not null.");

		if( !mIsInitialized )
		{
			// only init once
			go.AddComponent<ezDebugger>();
			mIsInitialized = true;
		}
	}

	void Awake()
	{
		mInstance = this;
		mIsInitialized = true;
	}
	
	// Use this for initialization
	void Start () {
		CalcViewSize();

        // init scroll point
        scrollPosition.x = intContentBoardLeft;
        scrollPosition.y = intContentBoardTop + 20;// 20 is the title bar's height
        
        mDefaultFontStyleRef = new GUIStyle();
        mDebuggerFontStyleRef = new GUIStyle();
        mDefaultFontStyleRef.normal.textColor = Color.white;
        mDefaultFontStyleRef.fontSize = intDefaultFontSize;
        mDefaultFontStyleRef.wordWrap = true;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        intScrollBoardMarginLeftRight *= 2;
#endif

        mDebuggerFontStyleRef.normal.textColor = Color.green;
        mDebuggerFontStyleRef.fontSize = intDebugInfoFontSize;
        mDebuggerFontStyleRef.wordWrap = true;

        // internal call
        InternalRegistCommand();
	}

	public static float fWindowWidthFactor = .5f;
	public static float fWindowHeightFactor = .5f;
	internal void CalcViewSize()
	{
		intWindowWidth = (int)(Screen.width * fWindowWidthFactor);
		intWindowHeight = (int)(Screen.height * fWindowHeightFactor);

		mWindowSizeRect.x = mWindowSizeRect.y = 0;
        mWindowSizeRect.x = Screen.width - intWindowWidth;

        mWindowSizeRect.width = intWindowWidth;
		mWindowSizeRect.height = intWindowHeight;

        intScrollWindowHeight = intWindowHeight - 20 - intContentBoardTop * 2 - intDebugInputHeight;

        // debug textarea
        mRectDebugTextArea.x = intContentBoardLeft;
		mRectDebugTextArea.y = intContentBoardTop + 20;// 20 is the title bar's height
		mRectDebugTextArea.width = intWindowWidth - intContentBoardLeft*2;
		intMinInputAreaHeight = intDebugInputHeight + intContentBoardTop + intComponentVerticalGap;
		mRectDebugTextArea.height = (int)(intWindowHeight - 20 - intContentBoardTop - intMinInputAreaHeight );
		
		// debug input whole rect
		mRectDebugInputSize.x = intContentBoardLeft;
		mRectDebugInputSize.y = mRectDebugTextArea.height + intContentBoardTop*2 + 20;
		mRectDebugInputSize.width = intWindowWidth - intContentBoardLeft*2;
		mRectDebugInputSize.height = intDebugInputHeight;
		
		mRectInputTextField.x = mRectDebugInputSize.x;
		mRectInputTextField.y = mRectDebugInputSize.y;
		mRectInputTextField.width = (int)(mRectDebugInputSize.width * .8f);
		mRectInputTextField.height = mRectDebugInputSize.height;
		
		// input button
		mRectEnterBtn.x = mRectDebugInputSize.x + mRectInputTextField.width + intComponentHorizontalGap;
		mRectEnterBtn.y = mRectDebugInputSize.y;
		mRectEnterBtn.width = intWindowWidth - mRectEnterBtn.x - intContentBoardLeft;
		mRectEnterBtn.height = mRectDebugInputSize.height;

        

    }

	public static bool mIsQuiteDebugger = false;
    private int intDraggableTitleHeight = 120;
    public static bool mIsDebuggerEditable = false;
    public static int intDebugInfoFontSize = 20;
    public static int intDefaultFontSize = 12;
    public static GUIStyle mDefaultFontStyleRef = null;
    public static GUIStyle mDebuggerFontStyleRef = null;
	void OnGUI()
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.025f;
        GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.025f;
#endif

        if ( !mIsQuiteDebugger )
			mWindowSizeRect = GUI.Window(0, mWindowSizeRect, OnMyWindowDrawHandler, mWindowTilteName+" - "+mDebuggerVersion );
	}

    private GUIStyle myStyle = new GUIStyle();
    public Vector2 scrollPosition = Vector2.zero;
    void OnMyWindowDrawHandler(int intWindowId)
	{
        //GUILayout.BeginVertical();

                // draw content
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(mRectDebugTextArea.width), GUILayout.Height(intScrollWindowHeight));

                    if ( mIsDebuggerEditable )
                        GUILayout.TextArea( mDebugMessageContent, mDebuggerFontStyleRef, GUILayout.Width(mRectDebugTextArea.width-intScrollBoardMarginLeftRight) );// new Rect(0, 0, this.mRectDebugTextArea.width - intScrollBoardMarginLeftRight, this.mRectDebugTextArea.height), 
                    else
                        GUILayout.Label( mDebugMessageContent, mDebuggerFontStyleRef, GUILayout.Width(mRectDebugTextArea.width - intScrollBoardMarginLeftRight));
            
                GUILayout.EndScrollView();

                // draw input & button
                mInputMessageContent = GUI.TextField(new Rect(mRectInputTextField.x, mRectInputTextField.y, mRectInputTextField.width, mRectInputTextField.height), mInputMessageContent);
                if (GUI.Button(new Rect(mRectEnterBtn.x, mRectEnterBtn.y, mRectEnterBtn.width, mRectEnterBtn.height), "OK"))
                {
                    OnEnterBtnClickHandler();
                }

        //GUILayout.EndVertical();

        
        // draggable
        GUI.DragWindow( new Rect(0, 0,intWindowWidth, intDraggableTitleHeight) );
	}

	//
	// When click enter button, process command
	//
	private void OnEnterBtnClickHandler()
	{
		//
		DealWithMessage( "ParseCMD", mInputMessageContent );
		ProcessCommand( mInputMessageContent );

		// commond: mInputMessageContent
		mInputMessageContent = "";// clear it
	}

	//
	// APIS
	//
	public static void Log(string message)
	{
		DealWithMessage( "Log", message );
	}

	public static void Warn(string message)
	{
		DealWithMessage( "Warn", message );
	}

	public static void Error(string message)
	{
		DealWithMessage( "Error", message );
	}

	public static void CustomLog(string type, string message)
	{
		DealWithMessage( type, message );
	}

	private static void DealWithMessage(string type, string message)
	{
		mDebugMessageBuilder.Append( type );
		mDebugMessageBuilder.Append( ": " ) ;
		mDebugMessageBuilder.Append( message );
		mDebugMessageBuilder.Append( "\n" );

		mDebugMessageContent = mDebugMessageBuilder.ToString();
	}

	//
	// cmd
	//
	private static Dictionary<string, List<ItemCommandEntity>> mMultiDictMapCommandHandler = new Dictionary<string, List<ItemCommandEntity>>();
	private static Dictionary<string, ItemCommandEntity> mSingleDictMapCommandHandler = new Dictionary<string, ItemCommandEntity>();
	public delegate void OnCommandHandlerDelegate(params System.Object[] args);
	public static void RegistCommand(string commandName, OnCommandHandlerDelegate onCommandHandler)
	{
		if( string.IsNullOrEmpty(commandName) || onCommandHandler == null )
		{
#if UNITY_EDITOR
			throw new Exception("argument is bad!");
#endif
		}

		if( mSingleDictMapCommandHandler.ContainsKey(commandName) )
		{
			ItemCommandEntity ic = mSingleDictMapCommandHandler[commandName];
			ic.handler = onCommandHandler;
		}
		else
		{
			ItemCommandEntity ic = new ItemCommandEntity();
			ic.commandName = commandName;
			ic.handler = onCommandHandler;
			mSingleDictMapCommandHandler.Add( commandName, ic );
		}
	}

	public static void RegistMultiCommand(string commandName, OnCommandHandlerDelegate onCommandHandler)
	{
		if( string.IsNullOrEmpty(commandName) || onCommandHandler == null )
		{
			#if UNITY_EDITOR
			throw new Exception("argument is bad!");
			#endif
		}

		List<ItemCommandEntity> listTmp = null;
		if( mMultiDictMapCommandHandler.ContainsKey(commandName) )
		{
			listTmp = mMultiDictMapCommandHandler[commandName];

			// push it
			ItemCommandEntity ic = new ItemCommandEntity();
			ic.commandName = commandName;
			ic.handler = onCommandHandler;

			listTmp.Add( ic );
		}
		else
		{
			listTmp = new List<ItemCommandEntity>(); 

			ItemCommandEntity ic = new ItemCommandEntity();
			ic.commandName = commandName;
			ic.handler = onCommandHandler;

			listTmp.Add( ic );
			mMultiDictMapCommandHandler.Add( commandName, listTmp );
		}
	}

	private static void RemoveCommand(string command)
	{
		if( mSingleDictMapCommandHandler.ContainsKey(command) ) 
		{
			ItemCommandEntity ic = mSingleDictMapCommandHandler[command];
			if( ic != null )
				ic.Clear();

			mSingleDictMapCommandHandler.Remove( command );
		}

		if( mMultiDictMapCommandHandler.ContainsKey(command) ) 
		{
			List<ItemCommandEntity> listTmp = mMultiDictMapCommandHandler[command];
			if( listTmp != null )
			{
				foreach(ItemCommandEntity ic in listTmp)
					ic.Clear();
			}

			mMultiDictMapCommandHandler.Remove( command );
		}
	}

	//
	// Process command
	//
	//
	private static void ProcessCommand(string expression)
	{
		string[] arrayCommand = expression.Split( new string[]{" "}, StringSplitOptions.RemoveEmptyEntries );
		string commandName = arrayCommand[0];
		commandName = commandName.Trim();

		List<System.Object> arrayParamsList = new List<System.Object>();
		int nums = arrayCommand.Length - 1;
		if( nums > 0 )
		{
			for(int k=1; k<=nums; k++)
			{
                string paramKey = arrayCommand[k];
                paramKey = paramKey.Trim();// clear it
                if (paramKey.Substring(0, 1) == "-")
                {
                    if( paramKey.Length >= 2 )
                        paramKey = paramKey.Substring(1);
                }
                arrayParamsList.Add(paramKey);// gather params
			}
		}

		// search command
		List<ItemCommandEntity> listTmp = new List<ItemCommandEntity>();
		if( mSingleDictMapCommandHandler.ContainsKey(commandName) )
		{
			ItemCommandEntity ic = mSingleDictMapCommandHandler[commandName];
			if( ic != null )
				listTmp.Add( ic );
		}
		if( mMultiDictMapCommandHandler.ContainsKey(commandName) )
		{
			List<ItemCommandEntity> listTmp2 = mMultiDictMapCommandHandler[commandName];
			if( listTmp2 != null )
				listTmp.AddRange( listTmp2 );
		}

		//
		if( arrayParamsList.Count > 0 )
		{
			foreach(ItemCommandEntity ic in listTmp)
			{
				ic.Excute( arrayParamsList.ToArray() );
			}
		}
		else
		{
			// not any params for  function
			foreach(ItemCommandEntity ic in listTmp)
			{
				ic.Excute();
			}
		}
	}

	private static void InternalRegistCommand()
	{
		RegistMultiCommand("clear", OnClearDebugMessageHandler);
		RegistMultiCommand("size", OnResizeWindowHandler);
		RegistMultiCommand("fullscreen", OnFullScreenWindowHandler);
		RegistMultiCommand("quite", OnQuiteDebuggerHandler);
		RegistMultiCommand("exit", OnQuiteDebuggerHandler);

        RegistMultiCommand("editable", OnEditAbleConfigHandler);

        // fontsize and fontcolor
        RegistMultiCommand("fontsize", OnConfigFontSizeHandler);
        RegistMultiCommand("fontcolor", OnConfigFontColorHandler);
    }

	private static void OnClearDebugMessageHandler(params System.Object[] args)
	{
		mDebugMessageContent = "";
		mDebugMessageBuilder.Remove(0, mDebugMessageBuilder.Length);
	}

	private static void OnResizeWindowHandler(params System.Object[] args)
	{
		if( fWindowHeightFactor > .4 )
		{
			fWindowHeightFactor = .3f;
			fWindowWidthFactor = .3f;
		}
		else
		{
			fWindowHeightFactor = .5f;
			fWindowWidthFactor = .5f;
		}

		if( mInstance != null )
			mInstance.CalcViewSize();
	}

	private static void OnFullScreenWindowHandler(params System.Object[] args)
	{
		if( fWindowHeightFactor >= 1.0f )
		{
			fWindowHeightFactor = .5f;
			fWindowWidthFactor = .5f;
		}
		else
		{
			fWindowHeightFactor = 1.0f;
			fWindowWidthFactor = 1.0f;
		}

		if( mInstance != null )
			mInstance.CalcViewSize();
	}

	private static void OnQuiteDebuggerHandler(params System.Object[] args)
	{
		mIsQuiteDebugger = true;
	}

    private static void OnEditAbleConfigHandler(params System.Object[] args)
    {
        mIsDebuggerEditable = !mIsDebuggerEditable;
    }

    private static void OnConfigFontSizeHandler(params System.Object[] args)
    {
        if (args.Length <= 0)
            return;

        Debug.Log("params="+args[0]);
        intDebugInfoFontSize = int.Parse(args[0].ToString());
        mDebuggerFontStyleRef.fontSize = intDebugInfoFontSize;
    }

    private static void OnConfigFontColorHandler(params System.Object[] args)
    {
        if (args.Length <= 0)
            return;

        Debug.Log("params=" + args[0]);
        string colorValue = args[0].ToString();
        Color customColor = Color.green;
        switch (colorValue)
        {
            case "white":
                customColor = Color.white;
                break;
            case "red":
                customColor = Color.red;
                break;
            case "blue":
                customColor = Color.blue;
                break;
            case "yellow":
                customColor = Color.yellow;
                break;
        }
        mDebuggerFontStyleRef.normal.textColor = customColor;
    }
}
class ItemCommandEntity
{
	public string commandName = "";
	public ezDebugger.OnCommandHandlerDelegate handler = null;

	//
	// excute
	//
	public void Excute(params System.Object[] args)
	{
		if( handler != null )
		{
			handler( args );
		}
	}

	//
	// delete any quote
	//
	public void Clear()
	{
		commandName = "";
		handler = null;
	}
}

