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

//will need to move ifdefs around for iOS - possibly just down to the static initializer where we setup the plugin

/// <summary>
/// AGS syncable string.
/// </summary>
public class AGSSyncableString : AGSSyncableStringElement
{
#if UNITY_IOS    
    #region external functions in iOS native code
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestString(string key, string val);
    [DllImport ("__Internal")]
    private static extern void _AmazonGCWSSetLatestStringMetadataAsJSON(string key, string val, string metadata);
    [DllImport ("__Internal")]
    private static extern bool _AmazonGCWSIsStringSet(string key);
    #endregion
    
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableString"/> class.
    /// </summary>
    /// <param name='JavaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableString(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }    

#if UNITY_ANDROID
    public AGSSyncableString(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableString"/> class.
    /// </summary>
    /// <param name='keyVal'>
    /// Key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableString(string keyVal, SyncableMethod syncableMethod) : base(keyVal, syncableMethod) {
        
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableString"/> class.
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
    public AGSSyncableString(AGSSyncableStringList listOwner, 
                            int listIndex, 
                            SyncableMethod syncableMethod) : base(listOwner, listIndex, syncableMethod) {
        
    } 
#endif    
    
    /// <summary>
    /// Set the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Set(string val){
#if UNITY_ANDROID
        javaObject.Call("set",val);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSSetLatestString(key, val);     
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
    public void Set(string val, Dictionary<string, string> metadata){
#if UNITY_ANDROID
            javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));    
#elif UNITY_IOS && !UNITY_EDITOR
        string metadataAsJSON = (null != metadata) ? metadata.toJson() : null;
        _AmazonGCWSSetLatestStringMetadataAsJSON(key, val, metadataAsJSON);     
#endif
    }

    /// <summary>
    /// returns true if a value is set
    /// </summary>
    /// <returns>
    /// bool indicating if this has been set
    /// </returns>
    public bool IsSet(){
#if UNITY_ANDROID
        return javaObject.Call<bool>("isSet");
#elif UNITY_IOS && !UNITY_EDITOR
        return _AmazonGCWSIsStringSet(key);     
#else
        return false;
#endif
    }
    
}
