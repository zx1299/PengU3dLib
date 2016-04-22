using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//
// Logger Record
// @author harry
// @date 04.22 2016
//
public class LoggerRecord {

    private static bool mIsIsInitialized = false;
    private static List<string> mCacheLogTextLine = new List<string>();
    private static StreamWriter sw = null;
    private static bool mIsOnlyWriteErrorLog = true;

    //
    // Install log listener
    //
    public static void InstallLogListener(bool isOnlyWriteErroLog)
    {
        if (mIsIsInitialized)
            return;

#if UNITY_5
        Application.logMessageReceived += OnLogCallBackHandler;
#else
        Application.RegisterLogCallback(OnLogCallBackHandler);
#endif

        // make a new log file
        DateTime dt = DateTime.Now;
        string fileName = dt.ToString("HH-mm-ss MM-dd-yyyy");
        string filePath = Application.persistentDataPath + "/" + fileName + ".log";
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        }

        sw = new StreamWriter(filePath, true, Encoding.UTF8 );

        mIsOnlyWriteErrorLog = isOnlyWriteErroLog;
        mIsIsInitialized = true;
    }
        
    //
    // Write type define
    //
    public static bool IsOnlyWriteErrorLog
    {
        get { return mIsOnlyWriteErrorLog; }
        set
        {
            mIsOnlyWriteErrorLog = value;
        }
    }

    static void OnLogCallBackHandler(string logString, string stackTrace, LogType type)
    {
        // write log message to a log file
        if (mIsOnlyWriteErrorLog)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
				if( !string.IsNullOrEmpty(logString) )
                	mCacheLogTextLine.Add(logString);

				if( !string.IsNullOrEmpty(stackTrace) )
                	mCacheLogTextLine.Add(stackTrace);
            }
        }
        else
        {
			if( !string.IsNullOrEmpty(logString) )
            	mCacheLogTextLine.Add(logString);

			if( !string.IsNullOrEmpty(stackTrace) )
            	mCacheLogTextLine.Add(stackTrace);
        }

        // Just write it
        while (mCacheLogTextLine.Count > 0)
        {
            string message = mCacheLogTextLine[0];
            sw.WriteLine(message);

            mCacheLogTextLine.RemoveAt(0);
        }
        sw.Flush();
    }

    public static void Clear()
    {
        if (sw != null)
            sw.Dispose();
        sw = null;

#if UNITY_5
        Application.logMessageReceived -= OnLogCallBackHandler;
#else
		Application.RegisterLogCallback(null);
#endif
    }




}
