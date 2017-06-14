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
/// AGS syncable list.
/// </summary>
public class AGSSyncableList : AGSSyncable
{
#if UNITY_IOS
   // The Android part of this plugin is able to safely pass around
    // AndroidJavaObjects, but iOS does not support anything similar.
    // For the iOS side of things then, it is safer to just look up
    // the information in the Objective C library.
    // This means the C# iOS part of this plugin needs to locally store
    // information it will need to retrieve the syncable number information.
    protected enum SyncableListBehavior {
        HighNumber,
        LowNumber,
        LatestNumber,
        LatestString,
    };
    
    // For iOS, this keeps track of the intended behavior of this list.
    protected readonly SyncableListBehavior listBehavior;
    
    #region external functions in iOS native code    
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetHighNumberListCount(string key);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLatestNumberListCount(string key);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLowNumberListCount(string key);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLatestStringListCount(string key);
    
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetHighNumberListMetadataAtIndex(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestNumberListMetadataAtIndex(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLowNumberListMetadataAtIndex(string listKey, int listIndex);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestStringListMetadataAtIndex(string listKey, int listIndex);
    
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSHighNumberListAddString(string key,string val); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSLatestNumberListAddString(string key,string val); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSLowNumberListAddString(string key,string val); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSLatestStringListAddString(string key,string val); 
    
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSHighNumberListAddStringAndMetadataAsJSON(string key,string val,string metadata); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSLatestNumberListAddStringAndMetadataAsJSON(string key,string val,string metadata); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSLowNumberListAddStringAndMetadataAsJSON(string key,string val,string metadata); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSLatestStringListAddStringAndMetadataAsJSON(string key,string val,string metadata); 
    
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSHighNumberListIsSet(string key); 
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSLatestNumberListIsSet(string key); 
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSLowNumberListIsSet(string key); 
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSLatestStringListIsSet(string key); 
    
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetHighNumberListMaxSize(string key); 
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLatestNumberListMaxSize(string key); 
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLowNumberListMaxSize(string key); 
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLatestStringListMaxSize(string key); 
    
    
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetHighNumberListMaxSize(string key,int size); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestNumberListMaxSize(string key,int size); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLowNumberListMaxSize(string key,int size); 
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestStringListMaxSize(string key,int size); 
    
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetHighNumberListTimestampAtIndex(string key, int listIndex); 
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLatestNumberListTimestampAtIndex(string key, int listIndex); 
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLowNumberListTimestampAtIndex(string key, int listIndex); 
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLatestStringListTimestampAtIndex(string key, int listIndex); 
    #endregion
    // These delegates are used to simplify the logic of selecting the external function to call.
    private System.Func<string /*key*/, /*out*/ int>                            getListCount = null;
    private System.Func<string /*key*/, int /*index*/, /*out*/ string>          getMetadataAtIndex = null;
    private System.Action<string /*key*/, string /*value*/>                     addStringToList = null;
    private System.Action<string /*key*/, string /*value*/, string /*metadata*/> addStringWithMetadataToList = null;
    private System.Func<string /*key*/, /*out*/ bool>                           isListSet = null;
    private System.Func<string /*key*/, /*out*/ int>                            getListMaxSize = null;
    private System.Action<string /*key*/, int /*max size*/>                     setListMaxSize = null;
    private System.Func<string /*key*/, int /*index*/, /*out*/ System.Int64>    getTimestampAtIndex = null;
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableList(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }

#if UNITY_ANDROID
    public AGSSyncableList(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS   
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableList"/> class.
    /// </summary>
    /// <param name='listKeyVal'>
    /// List key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableList(string listKeyVal, SyncableMethod syncableMethod) : base(listKeyVal, syncableMethod) {
        listBehavior = SyncableMethodToListBehavior(syncableMethod);
        InitializeNativeFunctionCalls();
    }
    
    /// <summary>
    /// Syncables the method to list behavior.
    /// </summary>
    /// <returns>
    /// The method to list behavior.
    /// </returns>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    SyncableListBehavior SyncableMethodToListBehavior(SyncableMethod syncableMethod) {
        switch(syncableMethod) {
        case SyncableMethod.getHighestNumber:
        case SyncableMethod.getHighNumberList:            
            return SyncableListBehavior.HighNumber;
        case SyncableMethod.getLatestNumber:
        case SyncableMethod.getLatestNumberList:
            return SyncableListBehavior.LatestNumber;
        case SyncableMethod.getLowestNumber:
        case SyncableMethod.getLowNumberList:
            return SyncableListBehavior.LowNumber;
        case SyncableMethod.getLatestString:
        case SyncableMethod.getLatestStringList:
            return SyncableListBehavior.LatestString;
        default:
            AGSClient.LogGameCircleError(string.Format("Unhandled AGSSyncableNumberList type {0}",syncableMethod.ToString()));
            // still need to return something if error logging is set to a level less than exception.
            return SyncableListBehavior.HighNumber;
        }        
    }
    
    /// <summary>
    /// Initializes the native function calls.
    /// </summary>
    void InitializeNativeFunctionCalls() {
#if !UNITY_EDITOR
        switch(listBehavior) {
        case SyncableListBehavior.HighNumber:
            setListMaxSize = _AmazonGCWSSetHighNumberListMaxSize; 
            getListMaxSize = _AmazonGCWSGetHighNumberListMaxSize;
            isListSet = _AmazonGCWSHighNumberListIsSet;
            addStringWithMetadataToList = _AmazonGCWSHighNumberListAddStringAndMetadataAsJSON;
            addStringToList = _AmazonGCWSHighNumberListAddString;
            getTimestampAtIndex = _AmazonGCWSGetHighNumberListTimestampAtIndex;
            getMetadataAtIndex = _AmazonGCWSGetHighNumberListMetadataAtIndex;
            getListCount = _AmazonGCWSGetHighNumberListCount;
            break;
        case SyncableListBehavior.LatestNumber:
            setListMaxSize = _AmazonGCWSSetLatestNumberListMaxSize; 
            getListMaxSize = _AmazonGCWSGetLatestNumberListMaxSize;
            isListSet = _AmazonGCWSLatestNumberListIsSet;
            addStringWithMetadataToList = _AmazonGCWSLatestNumberListAddStringAndMetadataAsJSON;
            addStringToList = _AmazonGCWSLatestNumberListAddString;
            getTimestampAtIndex = _AmazonGCWSGetLatestNumberListTimestampAtIndex;
            getMetadataAtIndex = _AmazonGCWSGetLatestNumberListMetadataAtIndex;
            getListCount = _AmazonGCWSGetLatestNumberListCount;
            break;
        case SyncableListBehavior.LowNumber:
            setListMaxSize = _AmazonGCWSSetLowNumberListMaxSize; 
            getListMaxSize = _AmazonGCWSGetLowNumberListMaxSize;
            isListSet = _AmazonGCWSLowNumberListIsSet;
            addStringWithMetadataToList = _AmazonGCWSLowNumberListAddStringAndMetadataAsJSON;
            addStringToList = _AmazonGCWSLowNumberListAddString;
            getTimestampAtIndex = _AmazonGCWSGetLowNumberListTimestampAtIndex;
            getMetadataAtIndex = _AmazonGCWSGetLowNumberListMetadataAtIndex;
            getListCount = _AmazonGCWSGetLowNumberListCount;
            break;
        case SyncableListBehavior.LatestString:
            setListMaxSize = _AmazonGCWSSetLatestStringListMaxSize; 
            getListMaxSize = _AmazonGCWSGetLatestStringListMaxSize;
            isListSet = _AmazonGCWSLatestStringListIsSet;
            addStringWithMetadataToList = _AmazonGCWSLatestStringListAddStringAndMetadataAsJSON;
            addStringToList = _AmazonGCWSLatestStringListAddString;
            getTimestampAtIndex = _AmazonGCWSGetLatestStringListTimestampAtIndex;
            getMetadataAtIndex = _AmazonGCWSGetLatestStringListMetadataAtIndex;
            getListCount = _AmazonGCWSGetLatestStringListCount;
            break;
        default:
            AGSClient.LogGameCircleError(string.Format("Unhandled Whispersync list behavior {0}",listBehavior.ToString()));
            break;
        }
#endif
    }
    
#endif    
    
    /// <summary>
    /// Sets the max size of this List..
    /// </summary>
    /// <remarks>
    /// Sets the max size of this List.  size must be positive
    /// and no greater than MAX_SIZE_LIMIT.  If size is smaller than the 
    /// current size of this SyncableNumberList in the cloud, then its size 
    /// will remain.  In other words, the size of SyncableNumberList can 
    /// never shrink.
    /// </remarks>
    /// <param name='size'>
    /// the max size of this List
    /// </param>    
    public void SetMaxSize(int size){
#if UNITY_ANDROID
        javaObject.Call( "setMaxSize", size );    
#elif UNITY_IOS
        if(null != setListMaxSize) {
            setListMaxSize(key,size);
        }
#endif
    }
    

    /// <summary>
    /// Gets the size of the max.
    /// </summary>
    /// <returns>
    /// The max size.
    /// </returns>
    public int GetMaxSize(){
#if UNITY_ANDROID
        return javaObject.Call<int>( "getMaxSize" );    
#elif UNITY_IOS
        if(null != getListMaxSize) {
            return getListMaxSize(key);
        }
        else {
            // any errors will have been reported when getListMaxSize was initialized.
            return 0;
        }
#else
        return 0;
#endif
    }
    
    /// <summary>
    /// bool indicating if a value is set
    /// </summary>
    /// <returns>
    /// bool indicating if a value is set
    /// </returns>
    public bool IsSet(){
#if UNITY_ANDROID
        return javaObject.Call<bool>( "isSet" );
#elif UNITY_IOS 
        if(null != isListSet) {
            return isListSet(key);
        }
        else {
            // any errors will have been reported when isListSet was initialized.
            return false;
        }
#else
        return false;
#endif
    }
    
    /// <summary>
    /// Add the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Add(String val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
        javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata)); 
#elif UNITY_IOS
        string metadataAsJSON = null != metadata ? metadata.toJson() : null;
        if(null != addStringWithMetadataToList) {
            addStringWithMetadataToList(key,val,metadataAsJSON);
        }
#endif
    }
    
    /// <summary>
    /// Add the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Add(String val){
#if UNITY_ANDROID
        javaObject.Call( "add", val );  
#elif UNITY_IOS
        if(null != addStringToList) {
            addStringToList(key,val);
        }
#endif
    }    
    

#if UNITY_IOS
    /// <summary>
    /// Gets the timestamp of the list value at the given index.
    /// </summary>
    /// <returns>
    /// The metadata at index.
    /// </returns>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    /// 
    public long GetTimestampAtIndex(int listIndex) {
        if(null != getTimestampAtIndex) {
            return getTimestampAtIndex(key, listIndex);
        }
        else {
            // any errors will have been reported when getTimestampAtIndex was initialized.
            return 0;
        }
    }
    
    /// <summary>
    /// Gets the metadata of the list value at the given index.
    /// </summary>
    /// <returns>
    /// The metadata at index.
    /// </returns>
    /// <param name='listIndex'>
    /// List index.
    /// </param>
    /// 
    public string GetMetadataAtIndex(int listIndex) {
        if(null != getMetadataAtIndex) {
            return getMetadataAtIndex(key, listIndex);
        }
        else {
            // any errors will have been reported when getMetadata was initialized.
            return null;
        }
    }
    
    /// <summary>
    /// Gets the size of the list.
    /// </summary>
    /// <returns>
    /// The list size.
    /// </returns>
     protected int getListSize() {
        // calls to native code do not work in editor
        if(null != getListCount) {
            return getListCount(key);
        }
        else {
            // any errors will have been reported when getListCount was initialized.
            return 0;
        }
    }
    
#endif
}
