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
/// Example implementation of a developer string for GameCircle Whispersync.
/// </summary>
public class AmazonGameCircleExampleWSDeveloperString {
    
    #region local variables
    // This contains the list of hashsets.
    private AGSSyncableDeveloperString developerString = null;
    // This contains the hashset of developer string keys
    private HashSet<string> developerStringKeys = null;
    #endregion
    
    #region Local const strings
    // This label is displayed if the passed in dataMap was null.
    private const string nullDataMapLabel = "Missing Whispersync DataMap";
    // This label is displayed if the developer string could not be initialized.
    private const string nullDeveloperString = "DeveloperString not yet initialized";
    // This label is for the button that retrieves the list of developer string keys
    private const string refreshDeveloperStringsButtonLabel = "Refresh Developer String Keys";
    
    // These are the labels for the different data available on the developer string.
    private const string cloudTimestampLabel = "Cloud Timestamp {0}";
    private const string cloudValueLabel = "Cloud Value {0}";
    private const string hashCodeLabel = "HashCode {0}";
    private const string timestampLabel = "Timestamp {0}";
    private const string valueLabel = "Value {0}";
    private const string inConflictLabel = "inConflict {0}";
    private const string isSetLabel = "isSet {0}";
    
    // This is the label for the button that sets a developer string value
    private const string setValueButtonLabel = "Set Value to \"{0}\"";
    
    // This is the key for the sample developer string
    private const string developerStringKey = "DeveloperString";
    // This is the value the developer string gets set to when the button is pressed
    private const string developerStringValue = "DeveloperStringValue";
    #endregion
    
    /// <summary>
    /// Draws an interface for this developer string.
    /// </summary>
    /// <param name='dataMap'>
    /// The GameCircle Data map.
    /// </param>
    public void DrawGUI(AGSGameDataMap dataMap) {
        // if the datamap is not available, display a message and leave.
        if(null == dataMap) {
            AmazonGUIHelpers.CenteredLabel(nullDataMapLabel);
            return;
        }
        
        // if the hash sets aren't initialize, attempt to initialize them.
        if(null == developerString) {
            InitializeDeveloperString(dataMap);   
        }
        
        // if the hash sets are still null, display a message and leave.
        if(null == developerString) {
            AmazonGUIHelpers.CenteredLabel(nullDeveloperString);
            return;   
        }        
        
        if(GUILayout.Button(refreshDeveloperStringsButtonLabel)) {
            developerStringKeys = dataMap.getDeveloperStringKeys();
        }
        if(null != developerStringKeys) {
            foreach(string key in developerStringKeys) {
                AmazonGUIHelpers.CenteredLabel(key);   
            }
        }
        
        // Display the data available on the developer string
        AmazonGUIHelpers.CenteredLabel(string.Format(cloudValueLabel,developerString.getCloudValue()));
        AmazonGUIHelpers.CenteredLabel(string.Format(valueLabel,developerString.getValue()));
        AmazonGUIHelpers.CenteredLabel(string.Format(inConflictLabel,developerString.inConflict().ToString()));
        AmazonGUIHelpers.CenteredLabel(string.Format(isSetLabel,developerString.isSet().ToString()));

        // This button sets the developer string to the developerStringValue.
        if(GUILayout.Button(string.Format(setValueButtonLabel,developerStringValue))) {
            developerString.setValue(developerStringValue);   
            // If there was a conflict, mark it was resolved, the value has been set.
            if(developerString.inConflict()) {
                developerString.markAsResolved();   
            }
        }
    }
    
    /// <summary>
    /// Initializes the developer string.
    /// </summary>
    /// <param name='dataMap'>
    /// Data map.
    /// </param>
    private void InitializeDeveloperString(AGSGameDataMap dataMap) {
        developerString = dataMap.getDeveloperString(developerStringKey);
    }
}
