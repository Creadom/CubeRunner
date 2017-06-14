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
using System.Collections;

/// <summary>
/// Amazon GameCircle example Whispersync accumulating number.
/// </summary>
public class AmazonGameCircleExampleWSAccumulatingNumber {
    
    #region local enums
    // The variable types that the accumulating number system supports.
    public enum AvailableAccumulatingNumberType {
        Int,
        Long,
        Double,
        String,
    };
    #endregion
    
    #region local variables
    // The type of the number saved by this accumulating number helper class.
    // Readonly so it can't be changed once it is set.
    private readonly AvailableAccumulatingNumberType type;
    private bool foldoutOpen = false;
    
    // The value stored by this menu starts as a nullable 
    // so the menu can attempt to retrieve the value the first time it is displayed.
    private double? valueAsDouble = null;
    private long? valueAsLong = null;
    private int? valueAsInt = null;
    private string valueAsString = null;    
    #endregion
    
    #region Local const strings
    // labels for modifying the value  of the accumulating number.
    private const string incrementButtonLabel = "Increment";
    private const string decrementButtonLabel = "Decrement";
    
    // A label to show the value of this accumulating number. 
    private const string accumulatingNumberValueLabel = "Accumulating number value: {0}";
    // A label for doubles with two points of decimal precision keeps the menu looking clean.
    private const string accumulatingNumberDoubleValueLabel = "Accumulating number value: {0,5:N1}";
    // This label is used when the accumulating number is not available yet.
    private const string noAccumulatingNumberLabel = "No value available.";
    
    // This error occurs when the value retrieved from Whispersync cannot be parsed to a number type.
    private const string unableToParseValueAsStringError = "Unable to parse accumulating number.";
    #endregion
    
    #region local const non-string values
    // These values are used when incrementing and decrementing the accumulating number.
    private const double doubleIncrementValue = 0.1f;
    private const int intIncrementValue = 1;
    private const long longIncrementValue = 1;
    private const string stringIncrementValue = "1";
    #endregion
    
    #region constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleWSAccumulatingNumber"/> class.
    /// </summary>
    /// <param name='newType'>
    /// Type to be used for the accumulating number.
    /// </param>
    public AmazonGameCircleExampleWSAccumulatingNumber(AvailableAccumulatingNumberType newType) {
        type = newType;
    }
    #endregion
    
    
    #region public functions
    /// <summary>
    /// Draws an interface for this syncable number helper.
    /// </summary>
    /// <param name='dataMap'>
    /// The GameCircle Data map.
    /// </param>
    public void DrawGUI(AGSGameDataMap dataMap) {
        // put a visual box around this syncable number
        GUILayout.BeginVertical(GUI.skin.box);
                
        // foldouts keep things clean, they let the interface remain
        // hidden until needed
        foldoutOpen = AmazonGUIHelpers.FoldoutWithLabel(foldoutOpen,SyncableVariableName());
        
        // Hide the menu if the foldout is not open.
        if(foldoutOpen) {
            // If the value has not been retrieved from Whispersync yet, retrieve it.
            if(!ValueAvailable()) {
                RetrieveAccumulatingNumberValue(dataMap);   
            }
            
            // Show the value.
            AmazonGUIHelpers.CenteredLabel(ValueLabel());   
                
            // This button increments the value.
            if(GUILayout.Button(incrementButtonLabel)) {
                IncrementValue(dataMap);                
            }
            
            // This button decrements the value.
            if(GUILayout.Button(decrementButtonLabel)) {
                DecrementValue(dataMap);                
            }
        }
        
        // Always make sure to match begin and end GUILayout functions.
        GUILayout.EndVertical();
    }
    #endregion
    
    #region Whispersync functions
    /// <summary>
    /// Retrieves the accumulating number value from Whispersync.
    /// </summary>
    /// <param name='dataMap'>
    /// Data map.
    /// </param>
    void RetrieveAccumulatingNumberValue(AGSGameDataMap dataMap) {
        // Retrieve the accumulating number value from Whispersync, based on its type.
        AGSSyncableAccumulatingNumber accumulatingNumber = dataMap.GetAccumulatingNumber(SyncableVariableName());
        switch(type) {
        case AvailableAccumulatingNumberType.Double:
            valueAsDouble = accumulatingNumber.AsDouble();
            break;
        case AvailableAccumulatingNumberType.Int:
            valueAsInt = accumulatingNumber.AsInt();
            break;
        case AvailableAccumulatingNumberType.Long:
            valueAsLong = accumulatingNumber.AsLong();
            break;
        case AvailableAccumulatingNumberType.String:
            valueAsString = accumulatingNumber.AsString();
            break;
        }
    }
    
    /// <summary>
    /// Increments the accumulating number value.
    /// </summary>
    /// <param name='dataMap'>
    /// Data map.
    /// </param>
    void IncrementValue(AGSGameDataMap dataMap) {
        // Increments the accumulating number value based on its type.
        AGSSyncableAccumulatingNumber accumulatingNumber = dataMap.GetAccumulatingNumber(SyncableVariableName());
        switch(type) {
        case AvailableAccumulatingNumberType.Double:
            accumulatingNumber.Increment(doubleIncrementValue);
            break;
        case AvailableAccumulatingNumberType.Int:
            accumulatingNumber.Increment(intIncrementValue);
            break;
        case AvailableAccumulatingNumberType.Long:
            accumulatingNumber.Increment(longIncrementValue);
            break;
        case AvailableAccumulatingNumberType.String:
            accumulatingNumber.Increment(stringIncrementValue);
            break;
        }
        
        // after the increment is complete, update the value locally.
        RetrieveAccumulatingNumberValue(dataMap);
    }
    
    /// <summary>
    /// Decrements the accumulating number value.
    /// </summary>
    /// <param name='dataMap'>
    /// Data map.
    /// </param>
    void DecrementValue(AGSGameDataMap dataMap) {
        // Increments the accumulating number value based on its type.
        AGSSyncableAccumulatingNumber accumulatingNumber = dataMap.GetAccumulatingNumber(SyncableVariableName());
        switch(type) {
        case AvailableAccumulatingNumberType.Double:
            accumulatingNumber.Decrement(doubleIncrementValue);
            break;
        case AvailableAccumulatingNumberType.Int:
            accumulatingNumber.Decrement(intIncrementValue);
            break;
        case AvailableAccumulatingNumberType.Long:
            accumulatingNumber.Decrement(longIncrementValue);
            break;
        case AvailableAccumulatingNumberType.String:
            accumulatingNumber.Decrement(stringIncrementValue);
            break;
        }
        
        // after the decrement is complete, update the value locally.
        RetrieveAccumulatingNumberValue(dataMap);
    }
    #endregion
    
    #region private functions
    /// <summary>
    /// A label for the value of this accumulating number.
    /// </summary>
    /// <returns>
    /// The value of this accumulating number in label form.
    /// </returns>
    string ValueLabel() {
        // if the value is not available, display a message mentioning that.
        if(!ValueAvailable()) {
            return noAccumulatingNumberLabel;
        }
        // display a label for the value based on its type.
        switch(type) {
        case AvailableAccumulatingNumberType.Double:
            return string.Format(accumulatingNumberDoubleValueLabel,valueAsDouble.Value);
        case AvailableAccumulatingNumberType.Int:
            return string.Format(accumulatingNumberValueLabel,valueAsInt.Value);
        case AvailableAccumulatingNumberType.Long:
            return string.Format(accumulatingNumberValueLabel,valueAsLong.Value);
        case AvailableAccumulatingNumberType.String:
            return string.Format(accumulatingNumberValueLabel,valueAsString);
            // unhandled cases are treated as if the value is not available.
        default:
            return noAccumulatingNumberLabel;
        }
    }
    
    /// <summary>
    /// Checks if the value of the accumulating number is available.
    /// </summary>
    /// <returns>
    /// The value is available.
    /// </returns>
    bool ValueAvailable() {
        // check if the value has been set, based on its type.
        switch(type) {
        case AvailableAccumulatingNumberType.Double:
            return valueAsDouble.HasValue;
        case AvailableAccumulatingNumberType.Int:
            return valueAsInt.HasValue;
        case AvailableAccumulatingNumberType.Long:
            return valueAsLong.HasValue;
        case AvailableAccumulatingNumberType.String:
            return !string.IsNullOrEmpty(valueAsString);
        default:
            // unhandled types are considered not available.
            return false;
        }
    }
                
    /// <summary>
    /// The variable name to use for retrieving and storing data in Whispersync.
    /// </summary>
    /// <returns>
    /// The variable name.
    /// </returns>
    string SyncableVariableName() {
        // The variable name is just the type.
        return type.ToString();            
    }
    #endregion
}
