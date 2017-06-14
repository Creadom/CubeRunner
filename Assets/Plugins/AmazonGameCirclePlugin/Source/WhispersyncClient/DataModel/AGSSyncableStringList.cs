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
/// AGS syncable string list.
/// </summary>
public class AGSSyncableStringList : AGSSyncableList
{
#if UNITY_IOS    
    #region external functions in iOS native code
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestStringListElementValue(string listKey, int listIndex);
    #endregion
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableStringList(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }
    
#if UNITY_ANDROID
    public AGSSyncableStringList(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    public AGSSyncableStringList(string listKeyVal, SyncableMethod syncableMethod) : base(listKeyVal, syncableMethod) {
        
    }
#endif
    /// <summary>
    /// Returns an immutable copy of the elements of this list as an array
    /// </summary>
    /// <returns>
    /// The values.
    /// </returns>
    public AGSSyncableString[] GetValues(){
#if UNITY_ANDROID
        AndroidJNI.PushLocalFrame(10);

        AndroidJavaObject[] records = javaObject.Call<AndroidJavaObject[]>("getValues");
        
        if(records == null || records.Length == 0){
            return null;
        }
        
        AGSSyncableString[] returnElements =
                new AGSSyncableString[records.Length];
        
        for( int i = 0; i < records.Length; ++i){
            returnElements[i] = new AGSSyncableString(records[i]);
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);

        return returnElements;
#elif UNITY_IOS && !UNITY_EDITOR
        // Data is pulled from the strings when they are accessed.
        // To access a string in a list, you need the list key, and the list index.
        int numberOfElements = getListSize();    
        AGSSyncableString [] elements = new AGSSyncableString[numberOfElements];
        for(int listIndex = 0; listIndex < numberOfElements; listIndex++) {
            elements[listIndex] = new AGSSyncableString(this, listIndex, method );
        }
        return elements;    
#else
        return null;
#endif
    }
    
    // calls to native code do not work in editor
#if UNITY_IOS && !UNITY_EDITOR
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
        return _AmazonGCWSGetLatestStringListElementValue(key, listIndex); 
    }
#endif
    
}
