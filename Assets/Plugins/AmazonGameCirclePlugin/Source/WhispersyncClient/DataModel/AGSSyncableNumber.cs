/**
 * © 2012-2013 Amazon Digital Services, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except in compliance with the License. A copy
 * of the License is located at
 *
 * http://aws.amazon.com/apache2.0/
 *
 * or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */
using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif


/// <summary>
/// AGS syncable number.
/// </summary>
public class AGSSyncableNumber : AGSSyncableNumberElement
{
#if UNITY_IOS
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberInt(string key, int val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberInt(string key, int val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberInt(string key, int val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberInt64(string key, System.Int64 val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberInt64(string key, System.Int64 val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberInt64(string key, System.Int64 val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberDouble(string key, double val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberDouble(string key, double val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberDouble(string key, double val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberString(string key, string val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberString(string key, string val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberString(string key, string val);
    
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSIsHighestNumberSet(string key);
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSIsLowestNumberSet(string key);
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSIsLatestNumberSet(string key);
       
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberWithMetadataJSONInt(string key, int val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberWithMetadataJSONInt(string key, int val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberWithMetadataJSONInt(string key, int val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberWithMetadataJSONInt64(string key, System.Int64 val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberWithMetadataJSONInt64(string key, System.Int64 val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberWithMetadataJSONInt64(string key, System.Int64 val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberWithMetadataJSONDouble(string key, double val, string metadataAsJSON);    
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberWithMetadataJSONDouble(string key, double val, string metadataAsJSON);   
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberWithMetadataJSONDouble(string key, double val, string metadataAsJSON);   
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighestNumberWithMetadataJSONString(string key, string val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowestNumberWithMetadataJSONString(string key, string val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberWithMetadataJSONString(string key, string val, string metadataAsJSON);
    
    // These delegates are used to simplify the logic of selecting the external function to call.
    private System.Action<string /*key*/,string /*number*/>         setNumberWithString = null;
    private System.Action<string /*key*/,int /*number*/>            setNumberWithInt = null;
    private System.Action<string /*key*/,double /*number*/>         setNumberWithDouble = null;
    private System.Action<string /*key*/,System.Int64 /*number*/>   setNumberWithLong = null;    
    
    private System.Action<string /*key*/,string /*number*/,string /*metadata*/>         setNumberWithStringAndMetadata = null;
    private System.Action<string /*key*/,int /*number*/,string /*metadata*/>            setNumberWithIntAndMetadata = null;
    private System.Action<string /*key*/,double /*number*/,string /*metadata*/>         setNumberWithDoubleAndMetadata = null;
    private System.Action<string /*key*/,System.Int64 /*number*/,string /*metadata*/>   setNumberWithLongAndMetadata = null;    
    
    private System.Func<string /*key*/,/*out*/ bool>    isNumberSet = null;
#endif    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumber"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableNumber(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }

#if UNITY_ANDROID
    public AGSSyncableNumber(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumber"/> class.
    /// </summary>
    /// <param name='numberKeyVal'>
    /// Number key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableNumber(string numberKeyVal, SyncableMethod syncableMethod) : base(numberKeyVal, syncableMethod) {
        InitializeNativeFunctionCalls();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumber"/> class.
    /// </summary>
    /// <param name='listOwner'>
    /// List owner.
    /// </param>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableNumber(AGSSyncableNumberList listOwner, 
                            int listIndex, 
                            SyncableMethod syncableMethod) : base(listOwner, listIndex, syncableMethod) {
        InitializeNativeFunctionCalls();
    }
    
    /// <summary>
    /// Initializes the native function calls.
    /// </summary>
    void InitializeNativeFunctionCalls() {
        // Does nothing in editor mode so these functions remain null.
#if !UNITY_EDITOR
        // Doing a single switch here keeps each individual function smaller and cleaner.
        switch(numberBehavior) {
        case SyncableNumberBehavior.Highest:
            setNumberWithString             = _AmazonGCWSSetHighestNumberString;
            setNumberWithInt                = _AmazonGCWSSetHighestNumberInt;
            setNumberWithDouble             = _AmazonGCWSSetHighestNumberDouble;
            setNumberWithLong               = _AmazonGCWSSetHighestNumberInt64;
            setNumberWithStringAndMetadata  = _AmazonGCWSSetHighestNumberWithMetadataJSONString;
            setNumberWithIntAndMetadata     = _AmazonGCWSSetHighestNumberWithMetadataJSONInt;
            setNumberWithDoubleAndMetadata  = _AmazonGCWSSetHighestNumberWithMetadataJSONDouble;
            setNumberWithLongAndMetadata    = _AmazonGCWSSetHighestNumberWithMetadataJSONInt64;
            break;
        case SyncableNumberBehavior.Lowest:
            setNumberWithString             = _AmazonGCWSSetLowestNumberString;
            setNumberWithInt                = _AmazonGCWSSetLowestNumberInt;
            setNumberWithDouble             = _AmazonGCWSSetLowestNumberDouble;
            setNumberWithLong               = _AmazonGCWSSetLowestNumberInt64;
            setNumberWithStringAndMetadata  = _AmazonGCWSSetLowestNumberWithMetadataJSONString;
            setNumberWithIntAndMetadata     = _AmazonGCWSSetLowestNumberWithMetadataJSONInt;
            setNumberWithDoubleAndMetadata  = _AmazonGCWSSetLowestNumberWithMetadataJSONDouble;
            setNumberWithLongAndMetadata    = _AmazonGCWSSetLowestNumberWithMetadataJSONInt64;
            break;
        case SyncableNumberBehavior.Latest:
            setNumberWithString             = _AmazonGCWSSetLatestNumberString;
            setNumberWithInt                = _AmazonGCWSSetLatestNumberInt;
            setNumberWithDouble             = _AmazonGCWSSetLatestNumberDouble;
            setNumberWithLong               = _AmazonGCWSSetLatestNumberInt64;
            setNumberWithStringAndMetadata  = _AmazonGCWSSetLatestNumberWithMetadataJSONString;
            setNumberWithIntAndMetadata     = _AmazonGCWSSetLatestNumberWithMetadataJSONInt;
            setNumberWithDoubleAndMetadata  = _AmazonGCWSSetLatestNumberWithMetadataJSONDouble;
            setNumberWithLongAndMetadata    = _AmazonGCWSSetLatestNumberWithMetadataJSONInt64;
            break;
        default:
            logUnhandledBehaviorError();
            break;
        }
#endif
    }
#endif    
    /// <summary>
    /// Set the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Set(long val){        
#if UNITY_ANDROID
        javaObject.Call( "set", val );
#elif UNITY_IOS
        // Java and C# seem to agree on what a "long" is.
        // However, C and C# were having problems, so this is
        // specifically calling it an Int64 for C.
        if(null != setNumberWithLong) {
            setNumberWithLong(key,(System.Int64) val);
        }                  
#endif
    }

      /// <summary>
      /// Set the specified val.
      /// </summary>
      /// <param name='val'>
      /// Value.
      /// </param>
    public void Set(double val){
#if UNITY_ANDROID
        javaObject.Call( "set", val );
#elif UNITY_IOS
        if(null != setNumberWithDouble) {
            setNumberWithDouble(key, val);
        }                 
#endif
    }    

    /// <summary>
    /// Set the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Set(int val){
#if UNITY_ANDROID
        javaObject.Call( "set", val );
#elif UNITY_IOS
        if(null != setNumberWithInt) {
            setNumberWithInt(key, val);
        }         
#endif
    }
    
     /// <summary>
     /// Set the specified val.
     /// </summary>
     /// <param name='val'>
     /// Value.
     /// </param>
    public void Set(string val){
#if UNITY_ANDROID
     javaObject.Call( "set", val );
#elif UNITY_IOS
        if(null != setNumberWithString) {
            setNumberWithString(key, val);
        }         
#endif
    }

    
    /// <summary>
    /// Set the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Set(long val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call( "set", val, DictionaryToAndroidHashMap(metadata) );
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != setNumberWithLongAndMetadata) {
            setNumberWithLongAndMetadata(key, (System.Int64) val,metadataAsJSON);
        }         
#endif
    }

    /// <summary>
    /// Set the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Set(double val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != setNumberWithDoubleAndMetadata) {
            setNumberWithDoubleAndMetadata(key, val, metadataAsJSON);
        }    
#endif
    }
    
    /// <summary>
    /// Set the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Set(int val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != setNumberWithIntAndMetadata) {
            setNumberWithIntAndMetadata(key, val, metadataAsJSON);
        }         
#endif
    }    

      /// <summary>
      /// Set the specified val and metadata.
      /// </summary>
      /// <param name='val'>
      /// Value.
      /// </param>
      /// <param name='metadata'>
      /// Metadata.
      /// </param>
    public void Set(string val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != setNumberWithStringAndMetadata) {
            setNumberWithStringAndMetadata(key, val, metadataAsJSON);
        }       
#endif
    }

      /// <summary>
      /// returns whether a value is set
      /// </summary>
      /// <returns>
      /// bool indicating if a value has been set
      /// </returns>
    public bool IsSet(){
#if UNITY_ANDROID
        return javaObject.Call<bool>("isSet");
#elif UNITY_IOS
        if(null != isNumberSet) {
            return isNumberSet(key);
        } 
        else {
            return false;
        }
#else
        return false;
#endif
    }
 
    
}
