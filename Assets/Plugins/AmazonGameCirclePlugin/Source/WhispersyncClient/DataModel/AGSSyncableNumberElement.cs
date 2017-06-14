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
/// AGS syncable number element.
/// </summary>
public class AGSSyncableNumberElement : AGSSyncableElement
{
#if UNITY_IOS
    // The Android part of this plugin is able to safely pass around
    // AndroidJavaObjects, but iOS does not support anything similar.
    // For the iOS side of things then, it is safer to just look up
    // the information in the Objective C library.
    // This means the C# iOS part of this plugin needs to locally store
    // information it will need to retrieve the syncable number information.
    protected enum SyncableNumberBehavior {
        Highest,
        Lowest,
        Latest,
    };
    protected readonly SyncableNumberBehavior numberBehavior;
            
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetHighestNumberInt(string key);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLowestNumberInt(string key);
    [DllImport ("__Internal")]
    private static extern int _AmazonGCWSGetLatestNumberInt(string key);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetHighestNumberInt64(string key);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLowestNumberInt64(string key);
    [DllImport ("__Internal")]
    private static extern System.Int64 _AmazonGCWSGetLatestNumberInt64(string key);
    [DllImport ("__Internal")]
    private static extern double _AmazonGCWSGetHighestNumberDouble(string key);
    [DllImport ("__Internal")]
    private static extern double _AmazonGCWSGetLowestNumberDouble(string key);
    [DllImport ("__Internal")]
    private static extern double _AmazonGCWSGetLatestNumberDouble(string key);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetHighestNumberString(string key);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLowestNumberString(string key);
    [DllImport ("__Internal")]
    private static extern string _AmazonGCWSGetLatestNumberString(string key);
    
    // These delegates are used to simplify the logic of selecting the external function to call.
    private System.Func<string /*key*/, /*out*/ int>                        getNumberAsInt = null;
    private System.Func<string /*key*/, /*out*/ System.Int64>               getNumberAsInt64 = null;
    private System.Func<string /*key*/, /*out*/ double>                     getNumberAsDouble = null;
    private System.Func<string /*key*/, /*out*/ string>                     getNumberAsString = null;
#endif
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumberElement"/> class.
    /// </summary>
    /// <param name='JavaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableNumberElement(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }    

#if UNITY_ANDROID
    public AGSSyncableNumberElement(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#elif UNITY_IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumberElement"/> class.
    /// </summary>
    /// <param name='elementKeyVal'>
    /// Element key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableNumberElement(string elementKeyVal, SyncableMethod syncableMethod) : base(elementKeyVal, syncableMethod) {
        
        numberBehavior = SyncableBehaviorFromSyncableMethod(syncableMethod);
        InitializeNativeFunctionCalls();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumberElement"/> class.
    /// </summary>
    /// <param name='listOwningThisNumber'>
    /// List owner.
    /// </param>
    /// <param name='indexInList'>
    /// List index.
    /// </param>
    /// <param name='numberKeyVal'>
    /// Number key value.
    /// </param>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    public AGSSyncableNumberElement(AGSSyncableNumberList listOwningThisNumber, 
                            int indexInList, 
                            SyncableMethod syncableMethod) : base(listOwningThisNumber, indexInList, syncableMethod) {
        
        numberBehavior = SyncableBehaviorFromSyncableMethod(syncableMethod);
        InitializeNativeFunctionCalls();
    }
    
    /// <summary>
    /// Initializes the behavior of the syncable number element for iOS.
    /// </summary>
    /// <param name='syncableMethod'>
    /// Syncable method.
    /// </param>
    SyncableNumberBehavior SyncableBehaviorFromSyncableMethod(SyncableMethod syncableMethod) {     
        switch(syncableMethod) {
        case SyncableMethod.getHighestNumber:
        case SyncableMethod.getHighNumberList:            
            return SyncableNumberBehavior.Highest;
        case SyncableMethod.getLatestNumber:
        case SyncableMethod.getLatestNumberList:
            return SyncableNumberBehavior.Latest;
        case SyncableMethod.getLowestNumber:
        case SyncableMethod.getLowNumberList:
            return SyncableNumberBehavior.Lowest;
        default:
            AGSClient.LogGameCircleError(string.Format("Unhandled SyncableNumberElement type {0}",syncableMethod.ToString()));
            return SyncableNumberBehavior.Latest;
        }   
    }
    
    /// <summary>
    /// Initializes the native function calls.
    /// </summary>
    void InitializeNativeFunctionCalls() {
        // Does nothing in editor mode so these functions remain null.
#if !UNITY_EDITOR
        // If the number is owned by a list, the list handles the calls to native code.
        if(null != listOwner && listIndex.HasValue && listOwner is AGSSyncableNumberList) {
            AGSSyncableNumberList asNumberList = listOwner as AGSSyncableNumberList;
            getNumberAsInt = (k) => { return asNumberList.GetValueAtIndexAsInt(listIndex.Value); };
            getNumberAsInt64 = (k) => { return asNumberList.GetValueAtIndexAsLong(listIndex.Value); };
            getNumberAsDouble = (k) => { return asNumberList.GetValueAtIndexAsDouble(listIndex.Value); };
            getNumberAsString = (k) => { return asNumberList.GetValueAtIndexAsString(listIndex.Value); };
        }
        else {
            // Doing a single switch here keeps each individual function smaller and cleaner.
            switch(numberBehavior) {
            case SyncableNumberBehavior.Highest:
                getNumberAsInt = _AmazonGCWSGetHighestNumberInt;
                getNumberAsInt64 = _AmazonGCWSGetHighestNumberInt64;
                getNumberAsDouble = _AmazonGCWSGetHighestNumberDouble;
                getNumberAsString = _AmazonGCWSGetHighestNumberString;
                break;
            case SyncableNumberBehavior.Lowest:
                getNumberAsInt = _AmazonGCWSGetLowestNumberInt;
                getNumberAsInt64 = _AmazonGCWSGetLowestNumberInt64;
                getNumberAsDouble = _AmazonGCWSGetLowestNumberDouble;
                getNumberAsString = _AmazonGCWSGetLowestNumberString;
                break;
            case SyncableNumberBehavior.Latest:
                getNumberAsInt = _AmazonGCWSGetLatestNumberInt;
                getNumberAsInt64 = _AmazonGCWSGetLatestNumberInt64;
                getNumberAsDouble = _AmazonGCWSGetLatestNumberDouble;
                getNumberAsString = _AmazonGCWSGetLatestNumberString;
                break;
            default:
                logUnhandledBehaviorError();
                break;
            }
        }
#endif
        
    }
    
#endif   
    
    /// <summary>
    /// returns long represnation of this element
    /// </summary>
    /// <returns>
    /// The long.
    /// </returns>
    public long AsLong(){
#if UNITY_ANDROID
        return javaObject.Call<long>("asLong");
#elif UNITY_IOS
        // If this is a number element in a list, access it through the list.
        if(null != getNumberAsInt64) {
            return getNumberAsInt64(key);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return 0;
        }
#else
        return 0;
#endif
    }

    /// <summary>
    /// returns double representation of this element
    /// </summary>
    /// <returns>
    /// The double.
    /// </returns>
    public double AsDouble(){
#if UNITY_ANDROID
        return javaObject.Call<double>("asDouble");
#elif UNITY_IOS
        // If this is a number element in a list, access it through the list.
        if(null != getNumberAsDouble) {
            return getNumberAsDouble(key);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return 0;
        }
#else
        return 0;
#endif
    }


    /// <summary>
    /// returns int representation of this element
    /// </summary>
    /// <returns>
    /// The int.
    /// </returns>
    public int AsInt(){
#if UNITY_ANDROID
        return javaObject.Call<int>("asInt");
#elif UNITY_IOS
        // If this is a number element in a list, access it through the list.
        if(null != getNumberAsInt) {
            return getNumberAsInt(key);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return 0;
        }
#else
        return 0;
#endif
    }    

       /// <summary>
       /// returns string representation of this element
       /// </summary>
       /// <returns>
       /// The string.
       /// </returns>
    public string AsString(){
#if UNITY_ANDROID
        return javaObject.Call<string>("asString");
#elif UNITY_IOS
        // If this is a number element in a list, access it through the list.
        if(null != getNumberAsString) {
            return getNumberAsString(key);
        }
        else {
            // any errors will have been reported when the delegate was initialized.
            return null;
        }
#else
        return null;
#endif
    }    
    
#if UNITY_IOS
    /// <summary>
    /// Logs the unhandled behavior error.
    /// </summary>
    protected void logUnhandledBehaviorError() {
        AGSClient.LogGameCircleError(string.Format("Whispsersync set number unhandled behavior {0}",numberBehavior.ToString()));
    }
#endif
}
