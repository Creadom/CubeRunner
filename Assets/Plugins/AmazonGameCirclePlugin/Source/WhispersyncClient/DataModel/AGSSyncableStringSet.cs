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
/// AGS syncable string set.
/// </summary>
public class AGSSyncableStringSet : AGSSyncable
{
#if UNITY_IOS    
    #region external functions in iOS native code
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSStringSetAdd(string listKey, string val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSStringSetAddWithJSONMetadata(string listKey, string val, string metadata);
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSStringSetContains(string listKey, string val);
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSStringSetIsSet(string listKey);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSStringSetGetValuesAsJSON(string listKey);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSStringSetGetTimestampForValue(string listKey, string valKey);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSStringSetGetMetadataAsJSONForValue(string listKey, string valKey);
        
    #endregion
#endif
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringSet"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableStringSet(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }

#if UNITY_ANDROID
    public AGSSyncableStringSet(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    public AGSSyncableStringSet(string setKeyVal, SyncableMethod syncableMethod) : base(setKeyVal, syncableMethod) {
        
    }
#endif

    /// <summary>
    /// Add the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Add(string val){
#if UNITY_ANDROID
        javaObject.Call("add",val);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSStringSetAdd(key,val);
#endif
    }

    /**
     * Adds a SyncableStringElement to this SyncableStringSet with the
     * given string value.  value cannot be null.
     * @param value The value to be added to this SyncableStringSet.
     * @param metadata the metadata associated with the SyncableStringElement
     *                 to be created.  Metadata can be null.
     * @throws java.lang.IllegalArgumentException if value is null or empty.
     * 
     * @see SyncableStringElement
     */
    public void Add(string val, Dictionary<string, string> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSStringSetAdd(key,val);
#endif
    }

    /// <summary>
    /// Get the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public AGSSyncableStringElement Get(string val){
        return GetAGSSyncable<AGSSyncableStringElement>(SyncableMethod.getStringSet,val);
    }

    /// <summary>
    /// Contains the specified val.
    /// </summary>
    /// <param name='val'>
    /// If set to <c>true</c> value.
    /// </param>
    public bool Contains(string val){
#if UNITY_ANDROID
        return javaObject.Call<bool>("contains",val);
#elif UNITY_IOS && !UNITY_EDITOR
        return _AmazonGCWSStringSetContains(key,val);
#else
        return false;
#endif
    }

    /// <summary>
    /// Ises the set.
    /// </summary>
    /// <returns>
    /// The set.
    /// </returns>
    public bool IsSet(){
#if UNITY_ANDROID
        return javaObject.Call<bool>("isSet");
#elif UNITY_IOS && !UNITY_EDITOR
        return _AmazonGCWSStringSetIsSet(key);
#else
        return false;
#endif
    }
    
    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <returns>
    /// The values.
    /// </returns>
    public HashSet<AGSSyncableStringElement> GetValues(){
#if UNITY_ANDROID    
        AndroidJNI.PushLocalFrame(10);

        //header states that this is non-null
        HashSet<AGSSyncableStringElement> returnSet = new HashSet<AGSSyncableStringElement>();

        AndroidJavaObject stringSet = javaObject.Call<AndroidJavaObject>( "getValues" );
        
        if(stringSet == null){
            return returnSet;
        }
        
        //get iterator from string set
        AndroidJavaObject iterator = stringSet.Call<AndroidJavaObject>( "iterator" );
        
        if(iterator == null){
            return returnSet;    
        }
        
        //could do size here...could...
        AndroidJavaObject jo;
        
        //iterate until it has been... iterated...
        while (  iterator.Call<bool>( "hasNext" ) ){
            jo = iterator.Call<AndroidJavaObject>( "next" );
            if(jo != null){
                returnSet.Add(new AGSSyncableStringElement(jo));
            }
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);

        return returnSet;    
#elif UNITY_IOS && !UNITY_EDITOR
        string values = _AmazonGCWSStringSetGetValuesAsJSON(key);
        if(string.IsNullOrEmpty(values)) {
            return null;
        }
        ArrayList jsonAsArrayList = values.arrayListFromJson();
        
        // Convert the array list to a hashset.
        HashSet<AGSSyncableStringElement> returnSet = new HashSet<AGSSyncableStringElement>();
        foreach(string jsonObject in jsonAsArrayList) {
            returnSet.Add(new AGSSyncableStringElement(this,jsonObject,method));   
        }
        return returnSet;
#else
        return default(HashSet<AGSSyncableStringElement>);
#endif
    }
    
#if UNITY_IOS
    
    /// <summary>
    /// Gets the timestamp for value.
    /// </summary>
    /// <returns>
    /// The timestamp for value.
    /// </returns>
    /// <param name='keyInSet'>
    /// Key in set.
    /// </param>
    public long GetTimestampForValue(string keyInSet) {
        return (long)_AmazonGCWSStringSetGetTimestampForValue(key,keyInSet);
    }
    
    /// <summary>
    /// Gets the metadata for value as JSON.
    /// </summary>
    /// <returns>
    /// The metadata for value as JSON.
    /// </returns>
    /// <param name='keyInSet'>
    /// Key in set.
    /// </param>
    public string GetMetadataForValueAsJSON(string keyInSet) {
        return _AmazonGCWSStringSetGetMetadataAsJSONForValue(key,keyInSet);
    }
#endif
}
