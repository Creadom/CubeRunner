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
/// Some helper functions for working with GUILayout
/// </summary>
public static class AmazonGUIHelpers {
    #region Local private constants
    // a darker color to show a difference between open and closed foldout buttons.
    private static readonly Color foldoutOpenColor = new Color(0.2f,0.2f,0.2f,1.0f);
    // The width of the foldout button
    private const float foldoutButtonWidth = 48;
    // The height of the foldout button
    private const float foldoutButtonHeight = 48;
    // The width of the min and max values for sliders
    private const float sliderMinMaxValuesLabelWidth = 75;    
    // a shared height for UI elements, so if a button is
    // replaced by a label or vice versa, the UI does not
    // change heights and positioning
    private const float uiHeight = 48;
    // Increasing the width of the slider makes it easier to touch.
    private const float uiSliderWidth = 48;
    // Increasing the height of the slider makes it easier to touch.
    private const float uiSliderHeight = 48;
    // A very wide scroll bar makes it easier to scroll
    // on touch screen devices.
    private const float uiScrollBarWidth = 48;
    // A little white space on each side of the menu
    // makes the menu look a little nicer.
    private const float menuPadding = 0.075f;
    #endregion
    
    
    #region Public GUI functions
    /// <summary>
    /// Modifies the passed in GUISkin to be touch friendly.
    /// </summary>
    /// <param name='skin'>
    /// Skin.
    /// </param>
    static public void SetGUISkinTouchFriendly(GUISkin skin) {        
        // Match these up in height, so if one is replaced by another, the entire UI does not shift.
        skin.button.fixedHeight = uiHeight;
        skin.label.fixedHeight = uiHeight;
        skin.textField.fixedHeight = uiHeight;
        skin.horizontalSlider.fixedHeight = uiHeight;
        skin.toggle.fixedHeight = uiHeight;
        
        // a bigger slider and slider thumb is easier to touch.
        skin.horizontalSlider.fixedHeight = uiSliderHeight;
        skin.horizontalSliderThumb.fixedHeight = uiSliderHeight;
        skin.horizontalSliderThumb.fixedWidth = uiSliderWidth;
        
        // Wider scrollbar to more easily touch.
        skin.verticalScrollbar.fixedWidth = uiScrollBarWidth;
        skin.verticalScrollbarThumb.fixedWidth = uiScrollBarWidth;
    }
    /// <summary>
    /// Displays a label with an anchor of MiddleCenter
    /// </summary>
    /// <param name='text'>
    /// Text.
    /// </param>
    /// <param name='options'>
    /// Options.
    /// </param>
    static public void CenteredLabel(string text, params GUILayoutOption[] options) {
        AnchoredLabel(text,TextAnchor.MiddleCenter,options);
    }
    
    /// <summary>
    /// Displays a label with adjustable anchoring
    /// </summary>
    /// <param name='text'>
    /// Text.
    /// </param>
    /// <param name='alignment'>
    /// Alignment.
    /// </param>
    /// <param name='options'>
    /// Options.
    /// </param>
    static public void AnchoredLabel(string text, TextAnchor alignment, params GUILayoutOption[] options) {
        TextAnchor oldAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = alignment;
        GUILayout.Label(text,options);       
        GUI.skin.label.alignment = oldAnchor;     
    }
        
    /// <summary>
    /// A button and a label to be used as a foldout. Replicates EditorGUILayout.foldout
    /// </summary>
    /// <returns>
    /// True if the foldout is open, false if not.
    /// </returns>
    /// <param name='currentValue'>
    /// The current value of this foldout.
    /// </param>
    /// <param name='label'>
    /// A label to display for the foldout.
    /// </param>
    static public bool FoldoutWithLabel(bool currentValue, string label) {
        GUILayout.BeginHorizontal();
        Color oldColor = GUI.color;
        if(currentValue) {
            GUI.color = foldoutOpenColor;
        }
        if(FoldoutButton()) {
            currentValue = !currentValue;   
        }
        GUI.color = oldColor;
        AnchoredLabel(label,TextAnchor.UpperCenter);
        // Make sure the label ends up properly centered
        GUILayout.Label(GUIContent.none,GUILayout.Width(foldoutButtonWidth));
        GUILayout.EndHorizontal();
        return currentValue;
    }
 
    /// <summary>
    /// Displays a label in the box skin, centered.
    /// </summary>
    /// <param name='text'>
    /// Text to display.
    /// </param>
    static public void BoxedCenteredLabel(string text) {
        GUILayout.BeginHorizontal(GUI.skin.box);
        CenteredLabel(text);
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Displays a slider that is centered.
    /// </summary>
    /// <returns>
    /// The adjusted slider value.
    /// </returns>
    /// <param name='currentValue'>
    /// Current value of the slider.
    /// </param>
    /// <param name='minValue'>
    /// Minimum value of the slider.
    /// </param>
    /// <param name='maxValue'>
    /// Max value of the slider.
    /// </param>
    /// <param name='valueDisplayString'>
    /// String to display below the slider.
    /// </param>
    static public float DisplayCenteredSlider(float currentValue, float minValue, float maxValue, string valueDisplayString) {
        GUILayout.BeginHorizontal();
        AnchoredLabel(string.Format(valueDisplayString,minValue),TextAnchor.UpperCenter,GUILayout.Width(sliderMinMaxValuesLabelWidth));
        GUILayout.BeginVertical();
        currentValue = GUILayout.HorizontalSlider(currentValue,minValue,maxValue);
        AnchoredLabel(string.Format(valueDisplayString,currentValue),TextAnchor.UpperCenter);
        GUILayout.EndVertical();
        AnchoredLabel(string.Format(valueDisplayString,maxValue),TextAnchor.UpperCenter,GUILayout.Width(sliderMinMaxValuesLabelWidth));
        GUILayout.EndHorizontal();   
        return currentValue;
    }
    
    /// <summary>
    /// Begins the menu layout for the Amazon example menus.
    /// Includes some slight padding on the sides to make the menu look a little nicer.
    /// </summary>
    static public void BeginMenuLayout() {
        // Begin a layout group the size of the screen
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width),GUILayout.Height(Screen.height));
        
        // Have a blank, narrow group on the left of the main content.
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * menuPadding));
        GUILayout.Label(GUIContent.none,GUILayout.Width(Screen.width * menuPadding));
        GUILayout.EndVertical();
        
        // Begin the main menu content, with the box skin.
        GUILayout.BeginVertical(GUI.skin.box);
    }
    
    /// <summary>
    /// Ends the menu layout for the Amazon example menu.
    /// </summary>
    static public void EndMenuLayout() {
        // End the box skin for the main menu layout.
        GUILayout.EndVertical();
        
        // Have another blank, narrow group on the right side of the main content,
        // that matches the size of the left side one, so the main content sits
        // in the middle center.
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * menuPadding));
        GUILayout.Label(GUIContent.none,GUILayout.Width(Screen.width * menuPadding));
        GUILayout.EndVertical();
        
        // End the full screen horizontal group.
        GUILayout.EndHorizontal();
    }
    #endregion
    
    #region Private helper functions
    /// <summary>
    /// Displays a button to be used for foldouts.
    /// </summary>
    /// <returns>
    /// Has the button be touched?
    /// </returns>
    private static bool FoldoutButton() {
        float oldButtonHeight = GUI.skin.button.fixedHeight;
        GUI.skin.button.fixedHeight = foldoutButtonHeight;
        bool result = GUILayout.Button(GUIContent.none,GUILayout.Width(foldoutButtonWidth),GUILayout.Height(foldoutButtonHeight));
        GUI.skin.button.fixedHeight = oldButtonHeight;
        return result;
    }
    #endregion
}
