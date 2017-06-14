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
/// A local cache of an element in the GameCircle number list.
/// </summary>
public class AmazonGameCircleExampleWSNumberListElementCache {
    
    #region variables
    private int valueAsInt = 0;
    private long valueAsLong = 0;
    private double valueAsDouble = 0;
    private string valueAsString = (0).ToString();
    private Dictionary<string,string> metadata = null;
    #endregion
        
    #region Local const strings
    // This is the label used to display a list element.
    // The expected input is # as int, # as long , # as double, # as string.
    private const string listElementLabel = "Int {0} : Long {1} : Double {2,5:N1} : String {3}";
    // this is the label that is before the metadata associated with a list element.
    private const string metadataLabel = "Metadata";
    // This label is shown for elements with no metadata
    private const string noMetadataAvailableLabel = "No metadata";
    #endregion
    
    #region constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleWSNumberListElementCache"/> class.
    /// </summary>
    /// <param name='intVal'>
    /// Int value.
    /// </param>
    /// <param name='longVal'>
    /// Long value.
    /// </param>
    /// <param name='doubleVal'>
    /// Double value.
    /// </param>
    /// <param name='stringVal'>
    /// String value.
    /// </param>
    /// <param name='elementMetadata'>
    /// Element metadata.
    /// </param>
    public AmazonGameCircleExampleWSNumberListElementCache(  int intVal, 
                                            long longVal, 
                                            double doubleVal,   
                                            string stringVal,
                                            Dictionary<string,string> elementMetadata) {
        valueAsInt = intVal;
        valueAsLong = longVal;
        valueAsDouble = doubleVal;
        valueAsString = stringVal;
        metadata = elementMetadata;
    }
    #endregion
    
    #region public functions
    /// <summary>
    /// Draws the element.
    /// </summary>
    public void DrawElement() {
        // putting a box around the information for this number element
        // makes it clear when you're looking at the next element.
        GUILayout.BeginVertical(GUI.skin.box);
        
        // display the value of the element in all supported types.
        string elementLabel = string.Format(listElementLabel,valueAsInt,valueAsLong,valueAsDouble,valueAsString);
        AmazonGUIHelpers.CenteredLabel(elementLabel);
       
        // display the metadata associated with the element.
        if(null != metadata && metadata.Count > 0) {
            AmazonGUIHelpers.CenteredLabel(metadataLabel);
            // display the entire dictionary
            foreach(KeyValuePair<string,string> metadataKvP in metadata) {
                AmazonGUIHelpers.CenteredLabel(metadataKvP.ToString());
            }
        }
        // if there was no metadata, display a label making note of that
        else {            
            AmazonGUIHelpers.CenteredLabel(noMetadataAvailableLabel);
        }
    
        GUILayout.EndVertical();
    }
    #endregion
    
}
