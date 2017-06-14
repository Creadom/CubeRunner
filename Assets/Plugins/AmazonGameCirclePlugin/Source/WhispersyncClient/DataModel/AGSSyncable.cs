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
/// AGS syncable.
/// </summary>
public class AGSSyncable : IDisposable {
    
    #region helper enums
    // this is a list of the syncable methods available.
    // This is an enum now instead of strings to make the iOS
    // part of this plugin simpler.
    public enum SyncableMethod {
        getDeveloperString,
        getHighestNumber,
        getLowestNumber,
        getLatestNumber,
        getHighNumberList,
        getLowNumberList,
        getLatestNumberList,
        getAccumulatingNumber,
        getLatestString,
        getLatestStringList,
        getStringSet,
        getMap,
    }
    
    // this is a list of the hashset methods available.
    // This is an enum now instead of strings to make the iOS
    // part of this plugin simpler.
    public enum HashSetMethod {
        getDeveloperStringKeys,
        getHighestNumberKeys,
        getLowestNumberKeys,
        getLatestNumberKeys,
        getHighNumberListKeys,
        getLowNumberListKeys,
        getLatestNumberListKeys,
        getAccumulatingNumberKeys,
        getLatestStringKeys,
        getLatestStringListKeys,
        getStringSetKeys,
        getMapKeys,
    }    
    #endregion
    
    protected AmazonJavaWrapper javaObject;
#if UNITY_IOS
    // There is no equivelant to the AndroidJavaObject for iOS.
    // It's generally safer to not carry around a pointer to an Objective C unmanaged object,
    // and instead let the Objective C portion of the plugin do the work of finding the information.
    protected readonly string key = null;
    protected readonly IntPtr iosObject;
    protected readonly SyncableMethod method;
    
    // These functions get a list of keys from Objective C, as a JSON formated string.
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetDeveloperStringKeysAsJSON();
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetAccumulatingNumberKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetHighestNumberKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetHighNumberListKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestNumberKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestNumberListKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestStringKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestStringListKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLowestNumberKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLowNumberListKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetMapKeysAsJSON(); 
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetStringSetKeysAsJSON(); 
    
    
#endif
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncable"/> class.
    /// </summary>
    /// <param name='jo'>
    /// Jo.
    /// </param>
    public AGSSyncable(AmazonJavaWrapper jo){
        javaObject = jo;
    }
#if UNITY_ANDROID
    public AGSSyncable(AndroidJavaObject jo){
        javaObject = new AmazonJavaWrapper(jo);
    }
#elif UNITY_IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncable"/> class.
    /// </summary>
    /// <param name='objectiveCPointer'>
    /// Objective C pointer.
    /// </param>
    public AGSSyncable(IntPtr objectiveCPointer) {
        iosObject = objectiveCPointer;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncable"/> class.
    /// </summary>
    public AGSSyncable() {
        
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncable"/> class.
    /// </summary>
    /// <param name='keyVal'>
    /// Key value of this Syncable object.
    /// </param>
    /// <param name='syncableMethod'>
    /// Method to be used to sync objects.
    /// </param>/
    public AGSSyncable(string keyVal, SyncableMethod syncableMethod) {
        key = keyVal;
        method = syncableMethod;
    }
#endif       
    
    
    /// <summary>
    /// try to enforce disposal references to AndrdoidJavaObjects
    /// upon completion
    /// </summary>
    public void Dispose(){
        if(javaObject != null){
            javaObject.Dispose();
        }
    }
    
#if UNITY_ANDROID        
    /// <summary>
    ///  Helper method for creating a java HashMap from a c# dictionary
    /// </summary>
    /// <param name="dictionary">dictionary to use for HashMap creation</param>
    /// <returns>AndroidJovaObject referencing a java HashMap</returns>
    protected AmazonJavaWrapper DictionaryToAndroidHashMap(Dictionary<String,String> dictionary){
        AndroidJNI.PushLocalFrame(10);

        AndroidJavaObject javaHashMap = new AndroidJavaObject("java.util.HashMap");
        
        //revert to manual JNI calls due to apparent bug in calling put on a hashmap object
        //from the AndroidJavaObject class
        IntPtr putMethod = AndroidJNIHelper.GetMethodID(javaHashMap.GetRawClass(), "put",
            "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
      
        object[] keyValSet = new object[2];
                
        foreach(KeyValuePair<string, string> kv in dictionary){

            using(AndroidJavaObject key = new AndroidJavaObject("java.lang.String", kv.Key)){
                using(AndroidJavaObject value = new AndroidJavaObject("java.lang.String", kv.Value)){
                    keyValSet[0] = key;
                    keyValSet[1] = value;
                    jvalue[] methodValues = AndroidJNIHelper.CreateJNIArgArray(keyValSet);
                    AndroidJNI.CallObjectMethod(javaHashMap.GetRawObject(),
                        putMethod, methodValues);
                }
            }
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
        return new AmazonJavaWrapper(javaHashMap);
    }    
#endif
    
    /// <summary>
    /// Gets the AGS syncable.
    /// </summary>
    /// <returns>
    /// The AGS syncable.
    /// </returns>
    /// <param name='method'>
    /// Method.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam>
    protected T GetAGSSyncable<T>(SyncableMethod method){
        return GetAGSSyncable<T>(method, null);
    }
    
    /// <summary>
    /// Gets the AGS syncable.
    /// </summary>
    /// <returns>
    /// The AGS syncable.
    /// </returns>
    /// <param name='method'>
    /// Method.
    /// </param>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam>
    protected T GetAGSSyncable<T>(SyncableMethod method, string key){
#if UNITY_ANDROID
        AndroidJavaObject jo;
        if(key != null){
            jo = javaObject.Call<AndroidJavaObject>( method.ToString(), key );
        }else{
            jo = javaObject.Call<AndroidJavaObject>( method.ToString() );    
        }
        if(jo != null){
            return (T)Activator.CreateInstance(typeof(T), new object[] { jo });
        }
         //return null or 0 as appropriate to the data type returned
         return default(T);  
#elif UNITY_IOS        
        return (T)Activator.CreateInstance(typeof(T), key, method);
#else
        //return null or 0 as appropriate to the data type returned
        return default(T);    
#endif
    }
    
    /// <summary>
    /// Gets the hash set.
    /// </summary>
    /// <returns>
    /// The hash set.
    /// </returns>
    /// <param name='method'>
    /// Method.
    /// </param>
    protected HashSet<string> GetHashSet(HashSetMethod method){
#if UNITY_ANDROID        
        AndroidJNI.PushLocalFrame(10);
        //never return null
        HashSet<string> returnSet = new HashSet<string>();

        //get the string set
        AndroidJavaObject stringSet = javaObject.Call<AndroidJavaObject>( method.ToString() );
        
        if(stringSet == null){
            return returnSet;
        }
        
        //get iterator from string set
        AndroidJavaObject iterator = stringSet.Call<AndroidJavaObject>( "iterator" );
        
        if(iterator == null){
            return returnSet;    
        }
        
        //iterate until it has been... iterated...
        while ( iterator.Call<bool>( "hasNext" ) ){        
            string key = iterator.Call<string>( "next" );
            returnSet.Add(key);
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);

        return returnSet;    
#elif UNITY_IOS
        // Get the HashSet from the Objective C library as a JSON formated string.
        string hashSetAsJSON = getHashSetAsJSON(method);
        if(null == hashSetAsJSON) {
            return null;
        }
        // Convert the string to an array list.
        ArrayList jsonAsArrayList = hashSetAsJSON.arrayListFromJson();
        // Convert the array list to a hashset.
        HashSet<string> returnSet = new HashSet<string>();
        foreach(string jsonObject in jsonAsArrayList) {
            returnSet.Add(jsonObject);   
        }
        return returnSet;
#else
        return default(HashSet<String>);
#endif
    }   
    
#if UNITY_IOS 
    /// <summary>
    /// Gets the hash set from the iOS library as JSON formatted string.
    /// </summary>
    /// <returns>
    /// The hash set as JSON formatted string.
    /// </returns>
    /// <param name='method'>
    /// The method to call.
    /// </param>
    string getHashSetAsJSON(HashSetMethod method) {
        // Having a method per hash set retrieval matches
        // the iOS SDK, and the Android half of the plugin's behavior.
        switch(method) {
        case HashSetMethod.getDeveloperStringKeys:
            return _AmazonGCWSGetDeveloperStringKeysAsJSON(); 
        case HashSetMethod.getAccumulatingNumberKeys:
            return _AmazonGCWSGetAccumulatingNumberKeysAsJSON(); 
        case HashSetMethod.getHighestNumberKeys:
            return _AmazonGCWSGetHighestNumberKeysAsJSON(); 
        case HashSetMethod.getHighNumberListKeys:
            return _AmazonGCWSGetHighNumberListKeysAsJSON(); 
        case HashSetMethod.getLatestNumberKeys:
            return _AmazonGCWSGetLatestNumberKeysAsJSON(); 
        case HashSetMethod.getLatestNumberListKeys:
            return _AmazonGCWSGetLatestNumberListKeysAsJSON(); 
        case HashSetMethod.getLatestStringKeys:
            return _AmazonGCWSGetLatestStringKeysAsJSON(); 
        case HashSetMethod.getLatestStringListKeys:
            return _AmazonGCWSGetLatestStringListKeysAsJSON(); 
        case HashSetMethod.getLowestNumberKeys:
            return _AmazonGCWSGetLowestNumberKeysAsJSON(); 
        case HashSetMethod.getLowNumberListKeys:
            return _AmazonGCWSGetLowNumberListKeysAsJSON(); 
        case HashSetMethod.getMapKeys:
            return _AmazonGCWSGetMapKeysAsJSON(); 
        case HashSetMethod.getStringSetKeys:
            return _AmazonGCWSGetStringSetKeysAsJSON(); 
        default:
            AGSClient.LogGameCircleError(string.Format("Unhandled hash set method {0}",method.ToString()));
            return null;
        }
    }
#endif
    

}
