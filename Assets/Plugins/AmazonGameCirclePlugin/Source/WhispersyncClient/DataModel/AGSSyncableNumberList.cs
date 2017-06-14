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
using System.Collections.Generic;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// AGS syncable number list.
/// </summary>
public class AGSSyncableNumberList : AGSSyncableList
{
#if UNITY_IOS    
    #region external functions in iOS native code
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddHighInt(string key, int val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLowInt(string key, int val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLatestInt(string key, int val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddHighInt64(string key, System.Int64 val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLowInt64(string key, System.Int64 val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLatestInt64(string key, System.Int64 val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddHighDouble(string key, double val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLowDouble(string key, double val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLatestDouble(string key, double val);
    
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddHighIntJSONMetadata(string key, int val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLowIntJSONMetadata(string key, int val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLatestIntJSONMetadata(string key, int val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddHighInt64JSONMetadata(string key, System.Int64 val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLowInt64JSONMetadata(string key, System.Int64 val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLatestInt64JSONMetadata(string key, System.Int64 val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddHighDoubleJSONMetadata(string key, double val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLowDoubleJSONMetadata(string key, double val, string metadataAsJSON);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSNumberListAddLatestDoubleJSONMetadata(string key, double val, string metadataAsJSON);
        
    [DllImport ("__Internal")]
    private static extern double _AmazonGCWSGetHighNumberListElementValueAsDouble(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern double _AmazonGCWSGetLatestNumberListElementValueAsDouble(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern double _AmazonGCWSGetLowNumberListElementValueAsDouble(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetHighNumberListElementValueAsInt64(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLatestNumberListElementValueAsInt64(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLowNumberListElementValueAsInt64(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetHighNumberListElementValueAsInt(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLatestNumberListElementValueAsInt(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLowNumberListElementValueAsInt(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetHighNumberListElementValueAsString(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestNumberListElementValueAsString(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLowNumberListElementValueAsString(string listKey, int listIndex);
    
    #endregion
    
    private System.Action<string /*key*/,int /*number*/>            addNumberAsInt = null;
    private System.Action<string /*key*/,System.Int64 /*number*/>   addNumberAsInt64 = null;
    private System.Action<string /*key*/,double /*number*/>         addNumberAsDouble = null;
    
    private System.Action<string /*key*/,int /*number*/, string /*metadata*/>           addNumberAsIntWithMetadata = null;
    private System.Action<string /*key*/,System.Int64 /*number*/, string /*metadata*/>  addNumberAsInt64WithMetadata = null;
    private System.Action<string /*key*/,double /*number*/, string /*metadata*/>        addNumberAsDoubleWithMetadata = null;
    
    private System.Func<string /*key*/, int /*index*/, /*out*/ int>            getValueAtIndexAsInt = null;
    private System.Func<string /*key*/, int /*index*/, /*out*/ System.Int64>   getValueAtIndexAsInt64 = null;
    private System.Func<string /*key*/, int /*index*/, /*out*/ double>         getValueAtIndexAsDouble = null;
    private System.Func<string /*key*/, int /*index*/, /*out*/ string>         getValueAtIndexAsString = null;
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumberList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableNumberList(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }

#if UNITY_ANDROID
    public AGSSyncableNumberList(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    public AGSSyncableNumberList(string listKeyVal, SyncableMethod syncableMethod) : base(listKeyVal, syncableMethod) {
        
        InitializeNativeFunctionCalls();
    }
    
    /// <summary>
    /// Initializes the native function calls.
    /// </summary>
    void InitializeNativeFunctionCalls() {
        // Does nothing in editor mode so these functions remain null.
#if !UNITY_EDITOR
        switch(listBehavior) {
        case SyncableListBehavior.HighNumber:
            addNumberAsInt = _AmazonGCWSNumberListAddHighInt;
            addNumberAsInt64 = _AmazonGCWSNumberListAddHighInt64;
            addNumberAsDouble = _AmazonGCWSNumberListAddHighDouble;
            addNumberAsIntWithMetadata = _AmazonGCWSNumberListAddHighIntJSONMetadata;
            addNumberAsInt64WithMetadata = _AmazonGCWSNumberListAddHighInt64JSONMetadata;
            addNumberAsDoubleWithMetadata = _AmazonGCWSNumberListAddHighDoubleJSONMetadata;
            getValueAtIndexAsInt = _AmazonGCWSGetHighNumberListElementValueAsInt;
            getValueAtIndexAsInt64 = _AmazonGCWSGetHighNumberListElementValueAsInt64;
            getValueAtIndexAsDouble = _AmazonGCWSGetHighNumberListElementValueAsDouble;
            getValueAtIndexAsString = _AmazonGCWSGetHighNumberListElementValueAsString;
            break;
        case SyncableListBehavior.LatestNumber:
            addNumberAsInt = _AmazonGCWSNumberListAddLatestInt;
            addNumberAsInt64 = _AmazonGCWSNumberListAddLatestInt64;
            addNumberAsDouble = _AmazonGCWSNumberListAddLatestDouble;
            addNumberAsIntWithMetadata = _AmazonGCWSNumberListAddLatestIntJSONMetadata;
            addNumberAsInt64WithMetadata = _AmazonGCWSNumberListAddLatestInt64JSONMetadata;
            addNumberAsDoubleWithMetadata = _AmazonGCWSNumberListAddLatestDoubleJSONMetadata;
            getValueAtIndexAsInt = _AmazonGCWSGetLatestNumberListElementValueAsInt;
            getValueAtIndexAsInt64 = _AmazonGCWSGetLatestNumberListElementValueAsInt64;
            getValueAtIndexAsDouble = _AmazonGCWSGetLatestNumberListElementValueAsDouble;
            getValueAtIndexAsString = _AmazonGCWSGetLatestNumberListElementValueAsString;
            break;
        case SyncableListBehavior.LowNumber:
            addNumberAsInt = _AmazonGCWSNumberListAddLowInt;
            addNumberAsInt64 = _AmazonGCWSNumberListAddLowInt64;
            addNumberAsDouble = _AmazonGCWSNumberListAddLowDouble;
            addNumberAsIntWithMetadata = _AmazonGCWSNumberListAddLowIntJSONMetadata;
            addNumberAsInt64WithMetadata = _AmazonGCWSNumberListAddLowInt64JSONMetadata;
            addNumberAsDoubleWithMetadata = _AmazonGCWSNumberListAddLowDoubleJSONMetadata;
            getValueAtIndexAsInt = _AmazonGCWSGetLowNumberListElementValueAsInt;
            getValueAtIndexAsInt64 = _AmazonGCWSGetLowNumberListElementValueAsInt64;
            getValueAtIndexAsDouble = _AmazonGCWSGetLowNumberListElementValueAsDouble;
            getValueAtIndexAsString = _AmazonGCWSGetLowNumberListElementValueAsString;
            break;
        default:
            AGSClient.LogGameCircleError(string.Format("Unhandled Whispersync list behavior {0}",listBehavior.ToString()));
            break;
        }
#endif
    }
    
#endif
    
      /// <summary>
      ///  Add the specified val. 
      /// </summary>
      /// <param name='val'>
      ///  Value. 
      /// </param>
    public void Add(long val){
#if UNITY_ANDROID
        javaObject.Call ( "add", val );    
#elif UNITY_IOS
        if(null != addNumberAsInt64) {
            addNumberAsInt64(key,val);
        }
#endif
    }

    /// <summary>
    ///  Add the specified val. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    public void Add(double val){
#if UNITY_ANDROID
        javaObject.Call ("add", val );    
#elif UNITY_IOS
        if(null != addNumberAsDouble) {
            addNumberAsDouble(key,val);
        }
#endif
    }

    /// <summary>
    ///  Add the specified val. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    public void Add(int val){
#if UNITY_ANDROID
        javaObject.Call( "add", val );    
#elif UNITY_IOS
        if(null != addNumberAsInt) {
            addNumberAsInt(key,val);
        }
#endif
    }
    
    /// <summary>
    ///  Add the specified val and metadata. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    /// <param name='metadata'>
    ///  Metadata. 
    /// </param>
    public void Add(long val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != addNumberAsInt64WithMetadata) {
            addNumberAsInt64WithMetadata(key,val,metadataAsJSON);
        }
#endif
    }    

    /// <summary>
    ///  Add the specified val and metadata. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    /// <param name='metadata'>
    ///  Metadata. 
    /// </param>
    public void Add(double val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != addNumberAsDoubleWithMetadata) {
            addNumberAsDoubleWithMetadata(key,val,metadataAsJSON);
        }
#endif
    }

       /// <summary>
       ///  Add the specified val and metadata. 
       /// </summary>
       /// <param name='val'>
       ///  Value. 
       /// </param>
       /// <param name='metadata'>
       ///  Metadata. 
       /// </param>
    public void Add(int val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != addNumberAsIntWithMetadata) {
            addNumberAsIntWithMetadata(key,val,metadataAsJSON);
        }
#endif
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <returns>
    /// The values.
    /// </returns>
    public AGSSyncableNumberElement[] GetValues(){
#if UNITY_ANDROID
        AndroidJNI.PushLocalFrame(10);
        AndroidJavaObject[] records = javaObject.Call<AndroidJavaObject[]>("getValues");
        
        if(records == null || records.Length == 0){
            return null;
        }
        
        AGSSyncableNumberElement[] returnElements =
                new AGSSyncableNumberElement[records.Length];
        
        for( int i = 0; i < records.Length; ++i){
            returnElements[i] = new AGSSyncableNumber(records[i]);
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);

        return returnElements;
        
#elif UNITY_IOS
        // Data is pulled from the numbers when they are accessed.
        // To access a number in a list, you need the list key, and the number index.
        int numberOfElements = getListSize();    
        AGSSyncableNumberElement [] elements = new AGSSyncableNumberElement[numberOfElements];
        for(int listIndex = 0; listIndex < numberOfElements; listIndex++) {
            elements[listIndex] = new AGSSyncableNumber(this, listIndex, method );
        }
        return elements;    
#else
        return null;
#endif
    }
    
    // calls to native code do not work in editor
#if UNITY_IOS
    /// <summary>
    /// Gets the value at index as string.
    /// </summary>
    /// <returns>
    /// The value at index as string.
    /// </returns>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    public string GetValueAtIndexAsString(int listIndex) {
        if(null != getValueAtIndexAsString) {
            return getValueAtIndexAsString(key,listIndex);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return null;
        }
    }
            
    /// <summary>
    /// Gets the value at index as long.
    /// </summary>
    /// <returns>
    /// The value at index as long.
    /// </returns>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    public long GetValueAtIndexAsLong(int listIndex) {
        if(null != getValueAtIndexAsInt64) {
            return getValueAtIndexAsInt64(key,listIndex);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return 0;
        }
    }
    /// <summary>
    /// Gets the value at index as int.
    /// </summary>
    /// <returns>
    /// The value at index as int.
    /// </returns>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    public int GetValueAtIndexAsInt(int listIndex) {
        if(null != getValueAtIndexAsInt) {
            return getValueAtIndexAsInt(key,listIndex);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return 0;
        }
    }
    
    /// <summary>
    /// Gets the value at index as double.
    /// </summary>
    /// <returns>
    /// The value at index as double.
    /// </returns>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    public double GetValueAtIndexAsDouble(int listIndex) {
        if(null != getValueAtIndexAsDouble) {
            return getValueAtIndexAsDouble(key,listIndex);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return 0;
        }
    }
    
#endif    
    
}
