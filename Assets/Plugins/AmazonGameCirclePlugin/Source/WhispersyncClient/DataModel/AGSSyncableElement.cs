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
///  Base syncable type that supports metadata and timestamp information
/// </summary>
public class AGSSyncableElement :AGSSyncable
{
#if UNITY_IOS
    // If an element is in a list,
    // it is best on iOS to retrieve it through its index in the list,
    // instead of carrying around unsafe pointers.
    protected readonly AGSSyncableList listOwner;
    protected readonly int? listIndex;
    // If this element is owned by a set, keep track of its owner.
#if !UNITY_EDITOR
    protected readonly AGSSyncableStringSet setOwner;
#endif
    
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetMetadataJSONHighestNumber(string key);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetMetadataJSONLatestNumber(string key);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetMetadataJSONLatestString(string key);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetMetadataJSONLowestNumber(string key);
    [DllImport ("__Internal")]
    private static extern Int64 _AmazonGCWSGetElementTimestampHighestNumber(string key);
    [DllImport ("__Internal")]
    private static extern Int64 _AmazonGCWSGetElementTimestampLatestNumber(string key);
    [DllImport ("__Internal")]
    private static extern Int64 _AmazonGCWSGetElementTimestampLatestString(string key);
    [DllImport ("__Internal")]
    private static extern Int64 _AmazonGCWSGetElementTimestampLowestNumber(string key);
    
    // These delegates are used to simplify the logic of selecting the external function to call.
    private System.Func<string /*key*/,/*out*/ string> getMetadata = null;
    private System.Func<string /*key*/,/*out*/ Int64> getTimestamp = null;
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableElement"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableElement(AmazonJavaWrapper javaObject) : base(javaObject){

    }

#if UNITY_ANDROID
    public AGSSyncableElement(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableElement"/> class.
    /// </summary>
    /// <param name='elementKeyVal'>
    /// Element key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableElement(string elementKeyVal, SyncableMethod syncableMethod) : base(elementKeyVal, syncableMethod) {
        listIndex = null;
        listOwner = null;
        InitializeNativeFunctionCalls();
#if !UNITY_EDITOR
        this.setOwner = null;
#endif
        
    }    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableElement"/> class.
    /// </summary>
    /// <param name='listOwningThisNumber'>
    /// List owning this number.
    /// </param>
    /// <param name='indexInList'>
    /// Index in list.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableElement(AGSSyncableList listOwningThisNumber, 
                            int indexInList, 
                            SyncableMethod syncableMethod) : base(null, syncableMethod) {
        
        listIndex = indexInList;
        listOwner = listOwningThisNumber;
        InitializeNativeFunctionCalls();
#if !UNITY_EDITOR
        this.setOwner = null;
#endif
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableElement"/> class.
    /// </summary>
    /// <param name='setOwner'>
    /// Set owner.
    /// </param>
    /// <param name='keyVal'>
    /// Key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableElement(AGSSyncableStringSet setOwner, 
                            string keyVal,
                            SyncableMethod syncableMethod) : base(keyVal, syncableMethod) {
        listIndex = null;
        listOwner = null;
        InitializeNativeFunctionCalls();
#if !UNITY_EDITOR
        this.setOwner = setOwner;
#endif
    } 
    
    /// <summary>
    /// Initializes the native function calls.
    /// </summary>
    void InitializeNativeFunctionCalls() {
#if !UNITY_EDITOR
        if(null != setOwner) {
            // Let the set get the timestamp of the element,
            // using the key of this element.
            getTimestamp = setOwner.GetTimestampForValue;
            getMetadata = setOwner.GetMetadataForValueAsJSON;
        }
        // else if the element is in a list,
        else if(null != listOwner && listIndex.HasValue) {
            // Create a local delegate that ignores the passed in key and instead
            // requests the information based on the list index.
            getTimestamp = (k) => { return listOwner.GetTimestampAtIndex(listIndex.Value); };
            getMetadata = (k) => { return listOwner.GetMetadataAtIndex(listIndex.Value); };
        }
        else {
            // Doing a single switch here keeps each individual function smaller and cleaner.
            switch(method) {
            case SyncableMethod.getHighestNumber:
                getTimestamp = _AmazonGCWSGetElementTimestampHighestNumber;
                getMetadata = _AmazonGCWSGetMetadataJSONHighestNumber;
                break;
            case SyncableMethod.getLatestNumber:
                getTimestamp = _AmazonGCWSGetElementTimestampLatestNumber;
                getMetadata = _AmazonGCWSGetMetadataJSONLatestNumber;
                break;
            case SyncableMethod.getLatestString:
                getTimestamp = _AmazonGCWSGetElementTimestampLatestString;
                getMetadata = _AmazonGCWSGetMetadataJSONLatestString;
                break;
            case SyncableMethod.getLowestNumber:
                getTimestamp = _AmazonGCWSGetElementTimestampLowestNumber;
                getMetadata = _AmazonGCWSGetMetadataJSONLowestNumber;
                break;
            case SyncableMethod.getAccumulatingNumber:
            case SyncableMethod.getHighNumberList:
            case SyncableMethod.getLatestNumberList:
            case SyncableMethod.getLatestStringList:
            case SyncableMethod.getLowNumberList:
            case SyncableMethod.getMap:
            case SyncableMethod.getStringSet:
                // these syncable method types are not actually SyncableElements, and do not have metadata.
                AGSClient.LogGameCircleError(string.Format("Whispersync SyncableElement is an unsupported method type {0}",method.ToString()));
                break;
            default:
                AGSClient.LogGameCircleError(string.Format("Unhandled Whispersync SyncableElement method {0}",method.ToString()));
                break;
            }
        }
#endif
    }
#endif        
    
    /// <summary>
    ///  The time in which this element was set as the number of seconds 
    /// elapsed since January 1, 1970, 00:00:00 GMT.
    /// </summary>
    /// <returns>time this element was set</returns>    
    public long GetTimestamp(){
#if UNITY_ANDROID
        return javaObject.Call<long>( "getTimestamp" );
#elif UNITY_IOS
        if(null != getTimestamp) {
            return getTimestamp(key);
        }
        else {
            return 0;
        }
#else
        return 0;
#endif
    }

    /// <summary>
    /// Optional metadata associated with this SyncableElement.  A
    /// non-null, unmodifiable map is returned.
    /// </summary>
    /// <returns>dictionary containing key,value map of metadata</returns>        
    public Dictionary<string,string> GetMetadata(){
#if UNITY_ANDROID        
        Dictionary<string,string> dictionary = new Dictionary<string, string>();
        
        AndroidJNI.PushLocalFrame(10);
        AndroidJavaObject javaMap = javaObject.Call<AndroidJavaObject>("getMetadata");
        if(javaMap == null){
            AGSClient.LogGameCircleError("Whispersync element was unable to retrieve metadata java map");
            return dictionary;    
        }
        
        AndroidJavaObject javaSet = javaMap.Call<AndroidJavaObject>("keySet");
        if(javaSet == null){
            AGSClient.LogGameCircleError("Whispersync element was unable to retrieve java keyset");
            return dictionary;    
        }

                
        AndroidJavaObject javaIterator = javaSet.Call<AndroidJavaObject>("iterator");
        if(javaIterator == null){
            AGSClient.LogGameCircleError("Whispersync element was unable to retrieve java iterator");
            return dictionary;    
        }
        
        string key, val;
        while( javaIterator.Call<bool>("hasNext") ){
            key = javaIterator.Call<string>("next");
            if(key != null){
                val = javaMap.Call<string>("get",key);
                if(val != null){
                    dictionary.Add (key,val);    
                }
            }    
        }    
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
        return dictionary;    
#elif UNITY_IOS
        string metadataAsJSON = null;
         if(null != getMetadata) {
             metadataAsJSON = getMetadata(key);
         }
        // if getMetadata was not available, or getMetadata returned a null / empty string,
        // return null.
        if(string.IsNullOrEmpty(metadataAsJSON)) {
            return null;
        }
        // Convert the string to a hashtable.
        Hashtable jsonAsHashtable = metadataAsJSON.hashtableFromJson();
        // Convert the hashtable to a ditionary.
        Dictionary<string,string> dictionary = new Dictionary<string, string>();
        foreach(string key in jsonAsHashtable.Keys) {
            dictionary.Add(key,jsonAsHashtable[key] as string);
        }
        return dictionary;
#else        
        return default(Dictionary<string,string>);
#endif
    }

}
