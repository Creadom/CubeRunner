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
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// AGS syncable string element.
/// </summary>
public class AGSSyncableStringElement : AGSSyncableElement
{
#if UNITY_IOS     
    #region external functions in iOS native code
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestStringElementValue(string key);
    #endregion
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringElement"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableStringElement(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }    
    
#if UNITY_ANDROID
    public AGSSyncableStringElement(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringElement"/> class.
    /// </summary>
    /// <param name='keyVal'>
    /// Key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableStringElement(string keyVal, SyncableMethod syncableMethod) : base(keyVal, syncableMethod) {
        
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringElement"/> class.
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
    public AGSSyncableStringElement(AGSSyncableStringList listOwner, 
                            int listIndex, 
                            SyncableMethod syncableMethod) : base(listOwner, listIndex, syncableMethod) {
        
    } 
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringElement"/> class.
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
    public AGSSyncableStringElement(AGSSyncableStringSet setOwner, 
                            string keyVal,
                            SyncableMethod syncableMethod) : base(setOwner, keyVal, syncableMethod) {
    } 
#endif
    
    
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>
    /// The value.
    /// </returns>
    public string GetValue(){
#if UNITY_ANDROID
        return javaObject.Call<string>("getValue");
#elif UNITY_IOS && !UNITY_EDITOR
        // If string element is in a set,
        if(null != setOwner) {
            // The value of a string element in a string set
            // is always going to be the same as its key.
            // Generally the usefulness of keeping a StringElement
            // from a string set around is to discover the metadata
            // and time it was set.
            return key;
        }
        // if the string element is in a list,
        else if(null != listOwner && listIndex.HasValue && listOwner is AGSSyncableStringList) {
            // The value of the string element can be obtained from the list, based on the element's index.
            return (listOwner as AGSSyncableStringList).GetValueAtIndexAsString(listIndex.Value);
        }
        // If the string element was not in a set or a list, then just get its value from native code.
        return _AmazonGCWSGetLatestStringElementValue(key);
#else
        return null;
#endif
    }    
    
}
