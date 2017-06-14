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
/// AGS syncable accumulating number.
/// </summary>
public class AGSSyncableAccumulatingNumber : AGSSyncable
{
#if UNITY_IOS
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberIncrementInt64(string key, System.Int64 delta);
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberIncrementInt(string key, int delta);
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberIncrementDouble(string key, double delta);
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberIncrementString(string key, string delta);
    
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberDecrementInt64(string key, System.Int64 delta);
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberDecrementInt(string key, int delta);
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberDecrementDouble(string key, double delta);
     [DllImport ("__Internal")]
     private static extern void _AmazonGCWSAccumulatingNumberDecrementString(string key, string delta);
    
     [DllImport ("__Internal")]
     private static extern System.Int64 _AmazonGCWSAccumulatingNumberAsInt64(string key);
     [DllImport ("__Internal")]
     private static extern int _AmazonGCWSAccumulatingNumberAsInt(string key);
     [DllImport ("__Internal")]
     private static extern double _AmazonGCWSAccumulatingNumberAsDouble(string key);
     [DllImport ("__Internal")]
     private static extern string _AmazonGCWSAccumulatingNumberAsString(string key);
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableAccumulatingNumber"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableAccumulatingNumber(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }

#if UNITY_ANDROID
    public AGSSyncableAccumulatingNumber(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    public AGSSyncableAccumulatingNumber(string numberKeyVal, SyncableMethod syncableMethod) : base(numberKeyVal, syncableMethod) {
        
    }
#endif    
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(long delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberIncrementInt64(key, (System.Int64) delta);
#endif
    }
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(double delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);    
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberIncrementDouble(key, delta);
#endif
    }
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(int delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberIncrementInt(key, delta);
#endif
    }
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(String delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);        
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberIncrementString(key, delta);
#endif
    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(long delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberDecrementInt64(key, (System.Int64) delta);
#endif

    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(double delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberDecrementDouble(key, delta);
#endif

    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(int delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberDecrementInt(key, delta);
#endif
    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(String delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#elif UNITY_IOS && !UNITY_EDITOR
        _AmazonGCWSAccumulatingNumberDecrementString(key, delta);
#endif
    }
 
    /// <summary>
    ///  gets current value as a long
    /// </summary>
    /// <returns>
    ///  long value
    /// </returns>
    public long AsLong(){
#if UNITY_ANDROID
        return javaObject.Call<long>("asLong");
#elif UNITY_IOS && !UNITY_EDITOR
        return (long) _AmazonGCWSAccumulatingNumberAsInt64(key);
#else
        return 0;
#endif
    }

    
    /// <summary>
    ///  gets current value as a double
    /// </summary>
    /// <returns>
    ///  double value
    /// </returns>    
    public double AsDouble(){
#if UNITY_ANDROID
        return javaObject.Call<double>("asDouble");
#elif UNITY_IOS && !UNITY_EDITOR
        return _AmazonGCWSAccumulatingNumberAsDouble(key);
#else
        return 0;
#endif
    }

    /// <summary>
    ///  gets current value as an int
    /// </summary>
    /// <returns>
    ///  int value
    /// </returns>
    public int AsInt(){
#if UNITY_ANDROID
        return javaObject.Call<int>("asInt");
#elif UNITY_IOS && !UNITY_EDITOR
        return _AmazonGCWSAccumulatingNumberAsInt(key);
#else
        return 0;
#endif
    }    

     /// <summary>
    ///  gets current value as a string
    /// </summary>
    /// <returns>
    ///  string value
    /// </returns>
    public string AsString(){
#if UNITY_ANDROID
        return javaObject.Call<string>("asString");
#elif UNITY_IOS && !UNITY_EDITOR
        return _AmazonGCWSAccumulatingNumberAsString(key);
#else
        return null;
#endif
    }        
}
