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
using System.Collections.Generic;

/// <summary>
/// Amazon GameCircle example Whispersync syncable number helper.
/// The number of savable numbers is very large, so this class
/// makes it easy to automate the testing of saving every
/// type of number, as well as every number behavior type.
/// </summary>
public class AmazonGameCircleExampleWSSyncableNumber {
    // Some helper enums so there can be a single function
    // for displaying a whispersync number, instead of one per number of behaviors times available types.
    #region local enums
    // Not using the one in AGSSyncable because it's iOS only (and should not be exposed to Android)
    // A number's behavior covers what "value" of a number's history is the most important. The highest, lowest, or latest. 
    public enum SyncableNumberBehavior {
        Highest,
        Lowest,
        Latest,
    };
    // The variable types that the syncable number system supports.
    public enum AvailableSyncableNumberType {
        Int,
        Long,
        Double,
        String,
    };
    #endregion
    
    #region local variables
    // The behavior of the number saved this syncable number helper class.
    // Readonly so it can't be changed once it is set.
    private readonly SyncableNumberBehavior behavior;
    // The type of the number saved by this syncable number helper class.
    // Readonly so it can't be changed once it is set.
    private readonly AvailableSyncableNumberType type;
    private bool foldoutOpen = false;
    
    // A local variable of every type the syncable number system can save.
    private int intNumber = 0;
    private long longNumber = 0;
    private double doubleNumber = 0;
    // note that the string is just a number stored as a string (and defaults to 0)
    // When it comes to dealing with the slider, the intNumber is temporarily used.
    private string stringNumber = ((int)0).ToString();
    
    // the dictionary of metadata associated with this syncable number.
    private Dictionary<string,string> metadataDictionary = null;
    #endregion
    
    #region Local const strings
    // The label (and number save variable name) for the behavior and type of a number.
    private const string behaviorAndTypeLabel = "{0}:{1}";
    
    // Some labels for displaying menus for the highest / lowest / latest numbers.
    private const string getValueLabel = "Get {0}";
    private const string setValueLabel = "Set {0}";
    private const string setWithMetadataValueLabel = "Set {0} with metadata";
    private const string numberSliderLabel = "{0}";
    
    // error messages.
    private const string unhandledSyncableNumberTypeError = "Whispersync unhandled syncable number type";
    
    // meta data information for the numbers, 
    // these are simple values to show the functionality.
    private const string metadataKey = "key";
    private const string metadataValue = "value";
    
    // The label on the button that retrieves metadata.
    private const string getMetadataButtonLabel = "Get metadata";
    // This is displayed when there is no metadata set on this number yet.
    private const string noMetaDataAvailableLabel = "No metadata set.";
    #endregion
    
    #region non-string constants
    // This provides a wide range of numbers to test whispersync data input.
    private const float lowestSliderValue = -10000;
    private const float highestSlidervalue = 10000;
    
    // This is a sample metadata dictionary, with a single key value pair.
    private readonly Dictionary<string,string> defaultMetadataDictionary = new Dictionary<string, string>() { { metadataKey, metadataValue } };
    #endregion
    
    #region constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleWhispersyncSyncableNumberHelper"/> class.
    /// </summary>
    /// <param name='newBehavior'>
    /// Behavior to be used for the syncable number.
    /// </param>
    /// <param name='newType'>
    /// Type to be used for the syncable number.
    /// </param>
    public AmazonGameCircleExampleWSSyncableNumber(  SyncableNumberBehavior newBehavior, 
                                                                    AvailableSyncableNumberType newType) {
        behavior = newBehavior;
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
        foldoutOpen = AmazonGUIHelpers.FoldoutWithLabel(foldoutOpen,BehaviorAndTypeAsString());
        
        if(foldoutOpen) {
            // draw the slider for this element, so a user can select a new number.
            DrawSlider();
            
            // The getValue button will retrieve the current save value for this number.
            // This syncable number's save name is Behavior:Type.
            if(GUILayout.Button(string.Format(getValueLabel,BehaviorAndTypeAsString()))) {
                using(AGSSyncableNumber syncableNumber = GetSyncableNumber(dataMap)) {
                    if(null != syncableNumber) {
                        GetSyncableValue(syncableNumber);
                    }
                }
            }
            
            // a blank label to space things out a bit.
            GUILayout.Label(GUIContent.none);
            
            // The setValue button will save this number with Whispersync.
            // This syncable number's save name is Behavior:Type.
            if(GUILayout.Button(string.Format(setValueLabel,BehaviorAndTypeAsString()))) {
                using(AGSSyncableNumber syncableNumber = GetSyncableNumber(dataMap)){
                    if(null != syncableNumber) {
                        SetSyncableValue(syncableNumber);
                    }
                }
            }
            
            // a blank label to space things out a bit.
            GUILayout.Label(GUIContent.none);
            
            // this button sets the syncable value, with some metadata.
            if(GUILayout.Button(string.Format(setWithMetadataValueLabel,BehaviorAndTypeAsString()))) {
                using(AGSSyncableNumber syncableNumber = GetSyncableNumber(dataMap)){
                    if(null != syncableNumber) {
                        SetSyncableValueWithMetadata(syncableNumber);
                    }
                }
            }
            
            // a blank label to space things out a bit.
            GUILayout.Label(GUIContent.none);
            
            // This button retrieves the metadata associated with this number.
            if(GUILayout.Button(string.Format(getMetadataButtonLabel,BehaviorAndTypeAsString()))) {
                using(AGSSyncableNumber syncableNumber = GetSyncableNumber(dataMap)){
                    if(null != syncableNumber) {
                        metadataDictionary = GetMetadata(syncableNumber);
                    }
                }
            }
            
            DisplayMetadata();
            
        }
        GUILayout.EndVertical();
    }
    #endregion
    
    #region private UI functions
    /// <summary>
    /// Displays the metadata associated with this number.
    /// </summary>
    void DisplayMetadata() {
        if(null != metadataDictionary && metadataDictionary.Count > 0) {
            // display every key value pair of metadata
            foreach(KeyValuePair<string,string> metadataKvP in metadataDictionary) {
                AmazonGUIHelpers.CenteredLabel(metadataKvP.ToString());
            }
        }
        else {
            // if there is no metadata available, display a label explaining that.
            AmazonGUIHelpers.CenteredLabel(noMetaDataAvailableLabel);   
        }   
    }
    
    /// <summary>
    /// Retrieves a composite string containing the behavior and type of this syncable number helper.
    /// </summary>
    /// <returns>
    /// Behavior and the type as a string.
    /// </returns>
    string BehaviorAndTypeAsString() {
        return string.Format(behaviorAndTypeLabel,behavior.ToString(),type.ToString());
    }
 
    /// <summary>
    /// Draws a slider for this syncable number helper.
    /// This allows the user to select a new value for the number stored.
    /// </summary>
    void DrawSlider() {
        switch(type) {
        case AvailableSyncableNumberType.Int:
            intNumber = (int)AmazonGUIHelpers.DisplayCenteredSlider(
                                (float)intNumber,lowestSliderValue,highestSlidervalue,numberSliderLabel);
            break;
        case AvailableSyncableNumberType.Double:
            doubleNumber = (double)AmazonGUIHelpers.DisplayCenteredSlider(
                                (float)doubleNumber,lowestSliderValue,highestSlidervalue,numberSliderLabel);
            break;
        case AvailableSyncableNumberType.Long:
            longNumber = (long)AmazonGUIHelpers.DisplayCenteredSlider(
                                (float)longNumber,lowestSliderValue,highestSlidervalue,numberSliderLabel);
            break;
        case AvailableSyncableNumberType.String:
            // It's easiest to just use the intNumber for the string's slider display.
            if(int.TryParse(stringNumber,out intNumber)) {
                intNumber = (int)AmazonGUIHelpers.DisplayCenteredSlider(
                                    (float)intNumber,lowestSliderValue,highestSlidervalue,numberSliderLabel);
                stringNumber = intNumber.ToString();
            }
            else {
                // if it couldn't be parsed, just display whatever value it was
                AmazonGUIHelpers.CenteredLabel(stringNumber);   
            }
            break;
        default:
            AGSClient.LogGameCircleWarning(unhandledSyncableNumberTypeError);
            break;
        }
    }
    #endregion
    
    #region private GameCircle functions
    /// <summary>
    /// Gets a GameCircle syncable number out of the data map.
    /// The SyncableNumber retrieved is based on the behavior of this syncable number helper.
    /// </summary>
    /// <returns>
    /// The syncable number.
    /// </returns>
    /// <param name='dataMap'>
    /// GameCircle data map.
    /// </param>
    AGSSyncableNumber GetSyncableNumber(AGSGameDataMap dataMap) {
        if(null == dataMap) {
            return null;
        }
        // using the behavior and type is a convenient way to create variable names.
        string variableName = BehaviorAndTypeAsString();
        switch(behavior) {
        case SyncableNumberBehavior.Highest:
            return dataMap.GetHighestNumber(variableName);
        case SyncableNumberBehavior.Lowest:
            return dataMap.GetLowestNumber(variableName);
        case SyncableNumberBehavior.Latest:
            return dataMap.GetLatestNumber(variableName);
        default:
            AGSClient.LogGameCircleWarning(unhandledSyncableNumberTypeError);
            return null;
        }
    }
    
    /// <summary>
    /// Gets the syncable value of a GameCircle syncable number associated with this syncableNumberHelper.
    /// Stores it in the local variable that matches this syncable number helper's type.
    /// </summary>
    /// <param name='syncableNumber'>
    /// Syncable number.
    /// </param>
    void GetSyncableValue(AGSSyncableNumber syncableNumber) {
        if(null == syncableNumber) {
            return;
        }
        switch(type) {
        case AvailableSyncableNumberType.Int:
            intNumber = syncableNumber.AsInt();
            break;
        case AvailableSyncableNumberType.Double:
            doubleNumber = syncableNumber.AsDouble();
            break;
        case AvailableSyncableNumberType.Long:
            longNumber = syncableNumber.AsLong();
            break;
        case AvailableSyncableNumberType.String:
            stringNumber = syncableNumber.AsString();
            break;
        default:
            AGSClient.LogGameCircleWarning(unhandledSyncableNumberTypeError);
            break;
        }
    }
    
    /// <summary>
    /// Sets the syncable value of a GameCircle syncable number associated with this syncableNumberHelper.
    /// </summary>
    /// <param name='syncableNumber'>
    /// Syncable number.
    /// </param>
    void SetSyncableValue(AGSSyncableNumber syncableNumber) {
        if(null == syncableNumber) {
            return;
        }
        switch(type) {
        case AvailableSyncableNumberType.Int:
            syncableNumber.Set(intNumber);
            break;
        case AvailableSyncableNumberType.Double:
            syncableNumber.Set(doubleNumber);
            break;
        case AvailableSyncableNumberType.Long:
            syncableNumber.Set(longNumber);
            break;
        case AvailableSyncableNumberType.String:
            syncableNumber.Set(stringNumber);
            break;
        default:
            AGSClient.LogGameCircleWarning(unhandledSyncableNumberTypeError);
            break;
        }
    }
    
    /// <summary>
    /// Sets the syncable value of a GameCircle syncable number associated with this syncableNumberHelper.
    /// Also sets the metadata for that value.
    /// </summary>
    /// <param name='syncableNumber'>
    /// Syncable number.
    /// </param>
    void SetSyncableValueWithMetadata(AGSSyncableNumber syncableNumber) {
        if(null == syncableNumber) {
            return;
        }
        switch(type) {
        case AvailableSyncableNumberType.Int:
            syncableNumber.Set(intNumber,defaultMetadataDictionary);
            break;
        case AvailableSyncableNumberType.Double:
            syncableNumber.Set(doubleNumber,defaultMetadataDictionary);
            break;
        case AvailableSyncableNumberType.Long:
            syncableNumber.Set(longNumber,defaultMetadataDictionary);
            break;
        case AvailableSyncableNumberType.String:
            syncableNumber.Set(stringNumber,defaultMetadataDictionary);
            break;
        default:
            AGSClient.LogGameCircleWarning(unhandledSyncableNumberTypeError);
            break;
        }
    }
    
    /// <summary>
    /// Gets the metadata associated with this number.
    /// </summary>
    /// <param name='syncableNumber'>
    /// Syncable number.
    /// </param>
    Dictionary<string,string> GetMetadata(AGSSyncableNumber syncableNumber) {
        if(null == syncableNumber) {
            return null;
        }
        return syncableNumber.GetMetadata();
    }    
    #endregion
}
