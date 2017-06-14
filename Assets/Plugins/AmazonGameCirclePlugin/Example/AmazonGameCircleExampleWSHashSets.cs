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
/// Container class for the example hash sets.
/// </summary>
public class AmazonGameCircleExampleWSHashSets {
    
    #region local variables
    // This contains the list of hashsets.
    private List<AmazonGameCircleExampleWSHashSet> hashSets = null;
    #endregion
    
    #region Local const strings
    // This label is displayed if the passed in dataMap was null.
    private const string nullDataMapLabel = "Missing Whispersync DataMap";
    // This label is displayed if the hash sets could not be initialized.
    private const string nullHashSets = "HashSets not yet initialized";
    // this label is on the button that refreshes all hash sets
    private const string refreshAllHashSetsButtonLabel = "Refresh All";
    #endregion

    #region public functions
    /// <summary>
    /// Draws an interface for this syncable number helper.
    /// </summary>
    /// <param name='dataMap'>
    /// Whispersync data map.
    /// </param>
    public void DrawGUI(AGSGameDataMap dataMap) {
        // if the datamap is not available, display a message and leave.
        if(null == dataMap) {
            AmazonGUIHelpers.CenteredLabel(nullDataMapLabel);
            return;
        }
        
        // if the hash sets aren't initialize, attempt to initialize them.
        if(null == hashSets) {
            InitializeHashSets(dataMap);   
        }
        
        // if the hash sets are still null, display a message and leave.
        if(null == hashSets) {
            AmazonGUIHelpers.CenteredLabel(nullHashSets);
            return;   
        }
        
        // This button refreshes all hash sets at once.
        if(GUILayout.Button(refreshAllHashSetsButtonLabel)) {
            RefreshAllHashSets();   
        }
        
        // display each hash set.
        foreach(AmazonGameCircleExampleWSHashSet hashSet in hashSets) {
            hashSet.DrawGUI();   
        }
    }
    #endregion
    
    #region private functions
    /// <summary>
    /// Initializes the hash sets.
    /// </summary>
    /// <param name='dataMap'>
    /// Whispersync data map.
    /// </param>
    void InitializeHashSets(AGSGameDataMap dataMap) {
        // bail out if the data map was not valid.
        if(null == dataMap) {
            return;
        }
        
        // Initialize the list of hash sets with all functions on the data map that return a hash set.
        hashSets = new List<AmazonGameCircleExampleWSHashSet>();
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetHighestNumberKeys",dataMap.GetHighestNumberKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetLowestNumberKeys",dataMap.GetLowestNumberKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetLatestNumberKeys",dataMap.GetLatestNumberKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetHighNumberListKeys",dataMap.GetHighNumberListKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetLowNumberListKeys",dataMap.GetLowNumberListKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetLatestNumberListKeys",dataMap.GetLatestNumberListKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetLatestStringKeys",dataMap.GetLatestStringKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetLatestStringListKeys",dataMap.GetLatestStringListKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetStringSetKeys",dataMap.GetStringSetKeys));
        hashSets.Add(new AmazonGameCircleExampleWSHashSet("GetMapKeys",dataMap.GetMapKeys));
    }
    
    /// <summary>
    /// Refreshs all hash sets.
    /// </summary>
    void RefreshAllHashSets() {
        // bail out if the data map was not valid.
        if(null == hashSets) {
            return;
        }
        
        foreach(AmazonGameCircleExampleWSHashSet hashSet in hashSets) {
            hashSet.Refresh();   
        }
    }
    #endregion
    
}
