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
/// Amazon GameCircle example Whispersync hash set.
/// </summary>
public class AmazonGameCircleExampleWSHashSet {
    
    #region class variables
    // the title of the hash set
    private string hashSetTitle = null;
    // the actual hash set
    private HashSet<string> hashSet = null;
    // the function to actually refresh this hash set
    private event System.Func<HashSet<string>> refreshHashSetFunction;
    // a GUI variable, to track if the foldout for this hash set is open.
    private bool foldoutOpen = false;
    #endregion
    
    #region Local const strings
    // A label to inform the user that the hash set they are viewing is actually empty.
    private const string emptyHashSetLabel = "Key list is empty";
    // A label for the button that refreshes the hash set.
    private const string refreshHashSetButtonLabel = "Refresh";
    #endregion
    
    #region constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleWSHashSet"/> class.
    /// </summary>
    /// <param name='title'>
    /// Title of this hash set.
    /// </param>
    /// <param name='refreshFunction'>
    /// The function to call to refresh this hash set.
    /// </param>
    public AmazonGameCircleExampleWSHashSet(string title, System.Func<HashSet<string>> refreshFunction) {
        hashSetTitle = title;
        
        if(null == refreshFunction) {
            return;
        }
        refreshHashSetFunction += refreshFunction;
        
        Refresh();
    }
    #endregion
    
    #region public functions
    /// <summary>
    /// Draws this hash set to the GUI
    /// </summary>
    public void DrawGUI() {
        // put a visual box around this syncable number
        GUILayout.BeginVertical(GUI.skin.box);
        
        // foldouts keep things clean, they let the interface remain
        // hidden until needed
        foldoutOpen = AmazonGUIHelpers.FoldoutWithLabel(foldoutOpen,hashSetTitle);
        
        // Hide the menu if the foldout is not open.
        if(foldoutOpen) {
            // this button lets the user fresh this hash set
            if(GUILayout.Button(refreshHashSetButtonLabel)) {
                Refresh();   
            }
            
            // if the hash set is empty, display a message.
            if(hashSet.Count == 0) {
                AmazonGUIHelpers.CenteredLabel(emptyHashSetLabel);
            }
            else {
                // display each entry in the hash set
                foreach(string hashSetString in hashSet) {
                    AmazonGUIHelpers.CenteredLabel(hashSetString);   
                }
            }
        }
            
        GUILayout.EndVertical();
    }
    
    /// <summary>
    /// Refresh the hash set.
    /// </summary>
    public void Refresh() {
        if(null == refreshHashSetFunction) {
            return;
        }
        hashSet = refreshHashSetFunction();
    }
    #endregion
}
