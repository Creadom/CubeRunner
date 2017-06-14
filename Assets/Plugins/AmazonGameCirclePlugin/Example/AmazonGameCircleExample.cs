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
/// Example implementation of AmazonGameCircle functionality.
/// </summary>
public class AmazonGameCircleExample : MonoBehaviour {


    #region Local const strings
    // Shows the GameCircle overlay
    private const string gameCircleOverlayButtonLabel = "Show GameCircle Overlay";
    // Shows the GameCircle sign in page
    private const string gameCircleSignInButtonLabel = "Show GameCircle Sign In";
    // Display if service is ready
    private const string isServiceReadyLabel = "Service ready: {0}";
    #endregion

    #region submenus
    // The menu for initializing GameCircle
    AmazonGameCircleExampleInitialization initializationMenu = new AmazonGameCircleExampleInitialization();

    // The list of example menus for GameCircle
    List<AmazonGameCircleExampleBase> gameCircleExampleMenus = new List<AmazonGameCircleExampleBase>();
    #endregion
    #region private variables
    bool initialized = false;
    #endregion

    #region UI variables
    // scroll position of the example UI
    Vector2 scroll = Vector2.zero;
    // Initialization status of the UI
    bool uiInitialized = false;
    // A local GUI skin, modified to be more touch friendly
    GUISkin localGuiSkin;
    // The skin the GUI had when OnGUI began
    GUISkin originalGuiSkin;
    #endregion

    #region Unity MonoBehaviour Overrides
    /// <summary>
    /// Unity / MonoBehaviour function for initialization.
    /// </summary>
    void Start() {
        // a separate, non-Mono initialization function
        // is safer for calling elsewhere in code.
        Initialize();
    }

    /// <summary>
    /// Unity / MonoBehaviour function for GUI behaviour.
    /// </summary>
    void OnGUI() {
        // Some initialization behaviour can only be called from within the OnGUI function.
        // initialize UI early returns if it is already initialized
        InitializeUI();

        // This menu has a local UI skin with buttons scaled up, and other touch enhancements.
        ApplyLocalUISkin();

        AmazonGUIHelpers.BeginMenuLayout();

        // Wrapping all of the menu in a scroll view allows the individual menu systems to not need to handle being off screen.
        scroll = GUILayout.BeginScrollView(scroll);

        // Display if the GameCircle plugin is ready..
        AmazonGUIHelpers.CenteredLabel(string.Format(isServiceReadyLabel, AGSClient.IsServiceReady()));

        // If GameCircle is not initialized, display the initialization menu.
        if(initializationMenu.InitializationStatus != AmazonGameCircleExampleInitialization.EInitializationStatus.Ready) {
                initializationMenu.DrawMenu();
        }
        else {

            // This button opens the generic GameCircle overlay.
            if(GUILayout.Button(gameCircleOverlayButtonLabel)) {
                AGSClient.ShowGameCircleOverlay();
            }

            // This button opens the GameCircle sign in page.
            if(GUILayout.Button(gameCircleSignInButtonLabel)) {
                AGSClient.ShowSignInPage();
            }

            // Once GameCircle is initialized, display all submenus, for achievements, leaderboards, and other GameCircle features.
            foreach(AmazonGameCircleExampleBase subMenu in gameCircleExampleMenus) {
                GUILayout.BeginVertical(GUI.skin.box);
                subMenu.foldoutOpen = AmazonGUIHelpers.FoldoutWithLabel(subMenu.foldoutOpen,subMenu.MenuTitle());
                if(subMenu.foldoutOpen) {
                    subMenu.DrawMenu();
                }
                GUILayout.EndVertical();
            }
        }

        GUILayout.EndScrollView();

        AmazonGUIHelpers.EndMenuLayout();

        // If the UI skin is not reverted at the end of the function,
        // any other OnGUI behavior might end up using the settings applied here.
        RevertLocalUISkin();
    }
    #endregion

    /// <summary>
    /// Initialize the AmazonGameCircleExample menu.
    /// </summary>
    void Initialize() {
        if(initialized) {
            return;
        }
        initialized = true;

        // Add all the GameCircle example submenus to the list of menus to display.
        gameCircleExampleMenus.Add(AmazonGameCircleExamplePlayer.Instance());
        gameCircleExampleMenus.Add(AmazonGameCircleExampleAchievements.Instance());
        gameCircleExampleMenus.Add(AmazonGameCircleExampleLeaderboards.Instance());
        gameCircleExampleMenus.Add(AmazonGameCircleExampleWhispersync.Instance());
    }

    #region UI Utility functions
    /// <summary>
    /// Initializes the UI for the GameCircle example menu. If already initialized, bails out.
    /// This function needs to be called from OnGUI to access GUI features.
    /// </summary>
    void InitializeUI() {
        if(uiInitialized) {
            return;
        }
        uiInitialized = true;
        localGuiSkin = GUI.skin;
        originalGuiSkin = GUI.skin;

        AmazonGUIHelpers.SetGUISkinTouchFriendly(localGuiSkin);

    }

    /// <summary>
    /// Applies the local user interface skin.
    /// Working in a local skin allows the example
    /// code to make UI changes without effecting
    /// other UI elements in Unity.
    /// </summary>
    void ApplyLocalUISkin() {
        GUI.skin = localGuiSkin;
    }

    /// <summary>
    /// Reverts the local user interface skin.
    /// Working in a local skin allows the example
    /// code to make UI changes without effecting
    /// other UI elements in Unity.
    /// </summary>
    void RevertLocalUISkin() {
        GUI.skin = originalGuiSkin;
    }
    #endregion
}
