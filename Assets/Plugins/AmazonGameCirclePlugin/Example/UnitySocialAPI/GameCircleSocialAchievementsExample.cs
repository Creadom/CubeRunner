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
using UnityEngine.SocialPlatforms;

/// <summary>
/// GameCircle example implementation of the GameCircleSocial plugin's achievement functionality.
/// </summary>
public class GameCircleSocialAchievementsExample {
    #region local variables
    // this allows the entire achievements menu section to be hidden behind a foldout menu
    bool achievementsMenuFoldout = false;
    
    // this allows the achievements section to be hidden behind a foldout menu
    bool achievementsFoldout = false;    
    // The list of achievements returned from the Social interface.
    IAchievement [] achievements = null;
    // The status of achievement retrieval.
    GameCircleSocialExample.AsyncOperationStatus achievementStatus = GameCircleSocialExample.AsyncOperationStatus.Inactive;
    
    // this allows the achievements descriptions section to be hidden behind a foldout menu
    bool achievementsDescriptionsFoldout = false;    
    // The list of achievement descriptions returned from the Social interface.
    IAchievementDescription [] achievementDescriptions = null;
    // The status of achievement description retrieval.
    GameCircleSocialExample.AsyncOperationStatus achievementDescriptionStatus = GameCircleSocialExample.AsyncOperationStatus.Inactive;
    #endregion
    
    #region local strings
    // The label for this menu
    private const string achievementMenuLabel = "Achievements Menu";
    // The label for the button that opens the achievement overlay
    private const string achievementOverlayLabel = "Achievement Overlay";
    // The label for the achievement section
    private const string achievementsLabel = "Achievements";
    // The label for the achievement description section
    private const string achievementDescriptionsLabel = "Achievement Descriptions";
    // the label for the button that retrieves achievement descriptions
    private const string retrieveDataButtonLabel = "Retrieve";
    // The label for when no achievement descriptions are available
    private const string noAchievementsLabel = "No achievements available"; 
    // The label for when no achievement descriptions are available
    private const string noAchievementDescriptionsLabel = "No achievement descriptions available"; 
    // The label for waiting on async operations
    private const string waitingLabel = "Waiting on async operation...";
    // The label for displaying the completion percent of an achievement.
    private const string achievementPercent = "{0,5:N1}% Completed";
    #endregion
        
    /// <summary>
    /// Draws the achievement menu.
    /// </summary>
    public void DrawAchievementsMenu() {
        // Use a foldout to hide this menu when the user does not want to see it.
        achievementsMenuFoldout = AmazonGUIHelpers.FoldoutWithLabel(achievementsMenuFoldout,achievementMenuLabel);
        if(!achievementsMenuFoldout) {
            return;
        }
        // This button opens the achievements overlay.
        if(GUILayout.Button(achievementOverlayLabel)) {
            Social.ShowAchievementsUI();   
        }
        
        // Show the achievements.
        GUILayout.BeginVertical(GUI.skin.box);
        ShowAchievements();
        GUILayout.EndVertical();
        
        // Show the achievement descriptions.
        GUILayout.BeginVertical(GUI.skin.box);
        ShowAchievementDescriptions();
        GUILayout.EndVertical();
    }
    /// <summary>
    /// Shows the achievements.
    /// </summary>
    void ShowAchievements() {
        // This foldout will hide the achievement list when the user does not want to see it.
        achievementsFoldout = AmazonGUIHelpers.FoldoutWithLabel(achievementsFoldout,achievementsLabel);
        if(!achievementsFoldout) {
            return;
        }
        switch(achievementStatus) {
        // If the achievements have not been retrieved yet,
        // display a button that begins the retrieval process.
        case GameCircleSocialExample.AsyncOperationStatus.Inactive:
            if(GUILayout.Button(retrieveDataButtonLabel)) {
                achievementStatus = GameCircleSocialExample.AsyncOperationStatus.Waiting;
                Social.LoadAchievements(AchievementsCallback);
            }
            break;
        // While waiting, display a simple message.
        case GameCircleSocialExample.AsyncOperationStatus.Waiting:
            AmazonGUIHelpers.CenteredLabel(waitingLabel);
            break;
        // If achievement descriptions were not retrieved, display a message that none are available.
        case GameCircleSocialExample.AsyncOperationStatus.Failed:
            AmazonGUIHelpers.CenteredLabel(noAchievementsLabel);
            break;
        // If the achievement descriptions were retrieved (and are available), display them.
        case GameCircleSocialExample.AsyncOperationStatus.Success:
            if(null == achievements || 0 == achievements.Length) {
                // If the list of achievements was null or empty, display a message that
                // no achievements are available.
                AmazonGUIHelpers.CenteredLabel(noAchievementDescriptionsLabel);
            }
            else {
                // display each achievement in a box, with the ID and percent complete on the same line.
                foreach(IAchievement achievement in achievements) {
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.BeginHorizontal();
                    AmazonGUIHelpers.CenteredLabel(achievement.id);
                    AmazonGUIHelpers.CenteredLabel(string.Format(achievementPercent,achievement.percentCompleted));
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            break;
        }           
    }
    
    /// <summary>
    /// Shows the achievement descriptions.
    /// </summary>
    void ShowAchievementDescriptions() {
        // This foldout hides the achievement descriptions until the user wishes to see them.
        achievementsDescriptionsFoldout = AmazonGUIHelpers.FoldoutWithLabel(   achievementsDescriptionsFoldout,
                                                                                                achievementDescriptionsLabel);
        if(!achievementsDescriptionsFoldout) {
            return;
        }
        switch(achievementDescriptionStatus) {
        // If the achievement descriptions have not been retrieved yet,
        // display a button that begins the retrieval process.
        case GameCircleSocialExample.AsyncOperationStatus.Inactive:
            if(GUILayout.Button(retrieveDataButtonLabel)) {
                achievementDescriptionStatus = GameCircleSocialExample.AsyncOperationStatus.Waiting;
                Social.LoadAchievementDescriptions(AchievementDescriptionsCallback);
            }
            break;
        // While waiting, display a simple message.
        case GameCircleSocialExample.AsyncOperationStatus.Waiting:
            AmazonGUIHelpers.CenteredLabel(waitingLabel);
            break;
        // If achievement descriptions were not retrieved, display a message that none are available.
        case GameCircleSocialExample.AsyncOperationStatus.Failed:
            AmazonGUIHelpers.CenteredLabel(noAchievementDescriptionsLabel);
            break;
        // If the achievement descriptions were retrieved (and are available), display them.
        case GameCircleSocialExample.AsyncOperationStatus.Success:
            // If the achievement description list is null or empty, display a message.
            if(null == achievementDescriptions || 0 == achievementDescriptions.Length) {
                AmazonGUIHelpers.CenteredLabel(noAchievementDescriptionsLabel);
            }
            else {
                // Display each achievement description, each in its own box.
                // Have the ID and title on one line, and the description on the next.
                foreach(IAchievementDescription description in achievementDescriptions) {
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.BeginHorizontal();
                    AmazonGUIHelpers.CenteredLabel(description.id);
                    AmazonGUIHelpers.CenteredLabel(description.title);
                    GUILayout.EndHorizontal();
                    AmazonGUIHelpers.CenteredLabel(description.achievedDescription);
                    GUILayout.EndVertical();
                }
            }
            break;
        }            
    }
    
    #region callbacks
    /// <summary>
    /// Callback for retrieving achievements.
    /// </summary>
    /// <param name='descriptions'>
    /// Descriptions.
    /// </param>
    void AchievementsCallback(IAchievement [] achievements) {
        // If the received list of achievements was null, then
        // for the purposes of this sample code, retrieving achievements has failed.
        if(null == achievements) {
            achievementStatus = GameCircleSocialExample.AsyncOperationStatus.Failed;
            return;
        }
        achievementStatus = GameCircleSocialExample.AsyncOperationStatus.Success;
        this.achievements = achievements;
    }
    /// <summary>
    /// Callback for retrieving achievement descriptions.
    /// </summary>
    /// <param name='descriptions'>
    /// Descriptions.
    /// </param>
    void AchievementDescriptionsCallback(IAchievementDescription [] descriptions) {
        // If the received list of descriptions was null, then
        // for the purposes of this sample code, retrieving descriptionss has failed.
        if(null == descriptions) {
            achievementDescriptionStatus = GameCircleSocialExample.AsyncOperationStatus.Failed;
            return;
        }
        achievementDescriptionStatus = GameCircleSocialExample.AsyncOperationStatus.Success;
        achievementDescriptions = descriptions;
    }
    #endregion
}
