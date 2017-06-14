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
/// Amazon GameCircle example implementation of Achievement functionality.
/// </summary>
public class AmazonGameCircleExampleAchievements : AmazonGameCircleExampleBase {

    #region Achievements Variables
    // Key : Achievement ID, value: achievement submission status
    Dictionary<string,string> achievementsSubmissionStatus = new Dictionary<string, string>();
    // Key: Achievement ID, value: achievement submission message (any errors that occured)
    Dictionary<string,string> achievementsSubmissionStatusMessage = new Dictionary<string, string>();
    // Key: Achievement ID, value: if the achievement is folded out.
    Dictionary<string,bool> achievementsFoldout = new Dictionary<string, bool>();
    // Status messages for showing the status of achievement retrieval
    string requestAchievementsStatus = null;
    string requestAchievementsStatusMessage = null;
    // the list of achievements received from the GameCircle plugin.
    List<AGSAchievement> achievementList = null;
    // have the achievements been received from the GameCircle plugin?
    // used in case a null list was received from the success callback.
    bool achievementsReady = false;
    // Key: Player ID, Value: List of achievements (or error)
    Dictionary<string, List<string>> achievementsForFriend = new Dictionary<string, List<string>> ();

    // List of friends (used for getting player score or leaderboard percentiles).
    List<string> friendIds = null;
    // The status of getting friend info.
    string friendStatus = null;
    // Whether to have the foldout for friends expanded.
    bool showFriendsAchievements = false;


    // What time achievements were requested at.
    // Used to show a timer to show the menu is active, and is just waiting on a callback.
    System.DateTime achievementsRequestTime;
    // used to test that GameCircle fails gracefully with invalid achievement information
    AGSAchievement invalidAchievement;
    #endregion

    #region UI variables
    // define the regex once here to save on re-creating the regex
    // This regex is intended to add a newline after every third comma in a string.
    // See function addNewlineAfterThirdComma for full implementation of this regex.
    readonly System.Text.RegularExpressions.Regex addNewlineEveryThirdCommaRegex = new System.Text.RegularExpressions.Regex(@"(,([^,]+,[^,]+),)");
    // The group number to use with the regex for keeping the text between commas.
    const int betweenCommaRegexGroup = 2;
    #endregion

    #region Local const strings
    // The title of this menu
    private const string achievementsMenuTitle = "Achievements";
    // The label for the button that opens the achievements overlay
    private const string displayAchievementOverlayButtonLabel = "Achievements Overlay";
    // The label for showing individual achievements in the UI. {0} is the achievement ID
    private const string achievementProgressLabel = "Achievement \"{0}\"";
    // The label for the button to submit any progress updates for achievements.
    private const string submitAchievementButtonLabel = "Submit Achievement Progress";
    // Labels for showing the status of achievement progress submission after it completes.
    private const string achievementFailedLabel = "Achievement \"{0}\" failed with error:";
    private const string achievementSucceededLabel = "Achievement \"{0}\" uploaded successfully.";
    // displays the achievement progress as a percentage value.
    private const string achievementPercent = "{0}%";
    // The label for the button to request achievement information.
    private const string requestAchievementsButtonLabel = "Request Achievements";
    // The status message to display that achievement retrieval has begun.
    private const string requestingAchievementsLabel = "Requesting Achievements...";
    // The label to display if achievement retrieval fails.
    private const string requestAchievementsFailedLabel = "Request Achievements failed with error:";
    // The label to display if achievement retrieval succeeds.
    private const string requestAchievementsSucceededLabel = "Available Achievements";
    // The label to display if the list of retrieved achievements was empty.
    private const string noAchievementsAvailableLabel = "No Achievements Available";
    // The label to display how long it has been since achievements were requested.
    private const string achievementRequestTimeLabel = "{0,5:N1} seconds";
    // The label to display that information submission has began for an achievement.
    private const string submittingInformationString = "Submitting Achievement...";
    // If the error string passed in to the update achievements callback is null or empty, this is the error it is replaced with.
    private const string noErrorMessageReceived = "MISSING ERROR STRING";
    #endregion

    #region local const values (non-string)
    // The achievement system should be able to handle negative values, this helps test that.
    private const float achievementMinValue = -200;
    // achievements are "complete" at 100.0f. A max value of 200 allows us to test and make sure achievement submission does not fail over 100%
    private const float achievementMaxValue = 200;
    #endregion

    #region Instance management

    // Singleton instance.
    private static AmazonGameCircleExampleAchievements instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleAchievements"/> class.
    /// </summary>
    private AmazonGameCircleExampleAchievements() {

        AGSAchievementsClient.RequestAchievementsCompleted += OnRequestAchievementsCompleted;
        AGSAchievementsClient.RequestAchievementsForPlayerCompleted += OnRequestAchievementsForFriendCompleted;
        AGSAchievementsClient.UpdateAchievementCompleted += OnUpdateAchievementCompleted;
        AGSPlayerClient.RequestFriendIdsCompleted += OnRequestFriendsCompleted;

        // This is used to test the GameCircle plugin's behaviour with invalid achievement information.
        // This achievement ID should not be in your game's list, to test what happens with invalid achievements.
        invalidAchievement = new AGSAchievement();
        invalidAchievement.title = "Invalid Achievement Title";
        invalidAchievement.id = "Invalid Achievement ID";
        invalidAchievement.progress = 0;
    }

    public static AmazonGameCircleExampleAchievements Instance () {
        if (instance == null) {
            instance = new AmazonGameCircleExampleAchievements();
        }
        return instance;
    }

    #endregion

    #region base class implementation
    /// <summary>
    /// The title of the menu.
    /// </summary>
    /// <returns>
    /// The title of the menu.
    /// </returns>
    public override string MenuTitle() {
        return achievementsMenuTitle;
    }

    /// <summary>
    /// Draws the Achievements menu. Note that this must be called from an OnGUI function.
    /// </summary>
    public override void DrawMenu() {
        // This button will open the achievements overlay.
        if(GUILayout.Button(displayAchievementOverlayButtonLabel)) {
            AGSAchievementsClient.ShowAchievementsOverlay();
        }

        // If achievement information retrieval has not begun,
        // display a button to begin the functionality.
        if(string.IsNullOrEmpty(requestAchievementsStatus)) {
            // This button will begin retrieval of achievement information.
            if(GUILayout.Button(requestAchievementsButtonLabel)) {
                // start the clock, to track the progress of this async operation
                achievementsRequestTime = System.DateTime.Now;
                // set the request status message to show that achievement retrieval has begun.
                requestAchievementsStatus = requestingAchievementsLabel;
                // request the achievement list from the GameCircle plugin.
                AGSAchievementsClient.RequestAchievements();
            }
        }
        else {
            // once a request has been made for the list of achievements,
            // display the status message of that process.
            AmazonGUIHelpers.CenteredLabel(requestAchievementsStatus);
            if(!string.IsNullOrEmpty(requestAchievementsStatusMessage)) {
                AmazonGUIHelpers.CenteredLabel(requestAchievementsStatusMessage);
            }

            // If the achievements are not ready, display how long it has been since the request was put in
            // to make it clear this is an asynchronous operation.
            if(!achievementsReady) {
                AmazonGUIHelpers.CenteredLabel(string.Format(achievementRequestTimeLabel,(System.DateTime.Now - achievementsRequestTime).TotalSeconds));
            }
            // once achievement retrieval is successful, display the list of achievements.
            else {
                // If the achievement list was not empty, display it.
                if(null != achievementList && achievementList.Count > 0) {
                    foreach(AGSAchievement achievement in achievementList) {
                        DisplayAchievement(achievement);
                    }
                }
                else {
                    // You will only see this message if there is no list of achievements.
                    // This happens in two cases:
                    //      You have not added any achievements to your project on the GameCircle website.
                    //      You only have draft achievements, and the user you are testing with cannot see draft achievements.
                    AmazonGUIHelpers.CenteredLabel(noAchievementsAvailableLabel);
                }
                // display an "invalid" achievement to ensure that GameCircle handles invalid data properly.
                if(null != invalidAchievement) {
                    DisplayAchievement(invalidAchievement);
                }
            }
        }

        if (friendIds == null) {
            if (GUILayout.Button("Get Friends")) {
                AGSPlayerClient.RequestFriendIds();
            }
        }

        if (!string.IsNullOrEmpty (friendStatus)) {
            AmazonGUIHelpers.CenteredLabel(friendStatus);
        }

        if (friendIds != null) {
            GUILayout.BeginVertical(GUI.skin.box);

            showFriendsAchievements = AmazonGUIHelpers.FoldoutWithLabel(showFriendsAchievements, "Request Achievements For Friends");

            if (showFriendsAchievements) {
                foreach (string friendId in friendIds) {
                    if (GUILayout.Button(string.Format("Achievements for {0}", friendId))){
                        AGSAchievementsClient.RequestAchievementsForPlayer(friendId);
                    }
                    if (achievementsForFriend.ContainsKey(friendId)) {
                        foreach (string achievement in achievementsForFriend[friendId]) {
                            AmazonGUIHelpers.CenteredLabel(achievement);
                        }
                    }
                }


            }

            GUILayout.EndVertical();
        }

    }
    #endregion

    #region UI functions
    /// <summary>
    /// Displays an individual achievement.
    /// </summary>
    /// <param name='achievement'>
    /// The achievement to display.
    /// </param>
    void DisplayAchievement(AGSAchievement achievement) {
        // Place a box around each achievement, to make it clear in the UI
        // what controls are for what achievement.
        GUILayout.BeginVertical(GUI.skin.box);

        // If this achievement has not been added to the foldout dictionary, add it.
        if(!achievementsFoldout.ContainsKey(achievement.id)) {
            achievementsFoldout.Add(achievement.id,false);
        }

        // Display a foldout for this achievement.
        // Foldouts keep the menu tidy.
        achievementsFoldout[achievement.id] = AmazonGUIHelpers.FoldoutWithLabel(achievementsFoldout[achievement.id],string.Format(achievementProgressLabel,achievement.id));

        // If the foldout is open, display the achievement information.
        if(achievementsFoldout[achievement.id]) {
            // The controls for automatically word wrapping a label are not great,
            // so replacing every third comma in the string returned from the achievement's toString function
            // will allow for a cleaner display of each achievement's data
            AmazonGUIHelpers.AnchoredLabel(AddNewlineEveryThirdComma(achievement.ToString()),TextAnchor.UpperCenter);

            // if this achievement has no pending progress submissions, display information to submit an update.
            if(!achievementsSubmissionStatus.ContainsKey(achievement.id) || string.IsNullOrEmpty(achievementsSubmissionStatus[achievement.id])) {

                // Display a centered slider, with the minimum value on the left, and maximum value on the right.
                // This lets the user select a value for the achievement's progress.
                achievement.progress = AmazonGUIHelpers.DisplayCenteredSlider(achievement.progress,achievementMinValue,achievementMaxValue,achievementPercent);

                // This button submits an update to the achievement's progress to the GameCircle plugin.
                if(GUILayout.Button(submitAchievementButtonLabel)) {
                    // Update the status of this achievement to show submission has begun.
                    achievementsSubmissionStatus[achievement.id] = string.Format(submittingInformationString);
                    // Submit the achievement update to the GameCircle plugin.
                    AGSAchievementsClient.UpdateAchievementProgress(achievement.id, achievement.progress);
                }
            }
            else {
                // If the achievement update is in the process of being submitted, display the submission status.
                AmazonGUIHelpers.CenteredLabel(achievementsSubmissionStatus[achievement.id]);

                if(achievementsSubmissionStatusMessage.ContainsKey(achievement.id) && !string.IsNullOrEmpty(achievementsSubmissionStatusMessage[achievement.id])) {
                    AmazonGUIHelpers.CenteredLabel(achievementsSubmissionStatusMessage[achievement.id]);
                }
            }
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Adds a newline after every third comma in a string.
    /// It is difficult to decipher the intent of a regex from its definition,
    /// Containing the behavior of the regex within these functions makes the intent clear.
    /// </summary>
    /// <returns>
    /// a string similar to the passed in string, but with a newline after every third comma.
    /// </returns>
    /// <param name='stringToChange'>
    /// String to change.
    /// </param>
    string AddNewlineEveryThirdComma(string stringToChange) {
        return addNewlineEveryThirdCommaRegex.Replace(    stringToChange,
                                                        (regexMatchEvaluator) => string.Concat(",",regexMatchEvaluator.Groups[betweenCommaRegexGroup].Value,",\n"));
    }
    #endregion

    #region Callbacks

    private void OnRequestAchievementsCompleted ( AGSRequestAchievementsResponse response ) {
        if (response.IsError ()) {
            // Update the status to show the failure message, and the error.
            requestAchievementsStatus = requestAchievementsFailedLabel;
            requestAchievementsStatusMessage = response.error;
        } else {
            // Update the status message to success.
            requestAchievementsStatus = requestAchievementsSucceededLabel;
            // Store the list of achievements locally.
            achievementList = response.achievements;
            // Mark the achievements as ready for use.
            // This bool is used instead of checking if the achievement list is null,
            // because the passed in achievement list can be null.
            // In that case, the menu should update to show that achievements were received, but empty.
            achievementsReady = true;
        }
    }

    private void OnUpdateAchievementCompleted ( AGSUpdateAchievementResponse response ) {
        if (response.IsError()) {
            // Update the achievement submission status to show the error.
            achievementsSubmissionStatus[response.achievementId] = string.Format(achievementFailedLabel, response.achievementId);
            achievementsSubmissionStatusMessage[response.achievementId] = response.error;
        } else {
            // Update the status message to show that achievement submission was successful.
            achievementsSubmissionStatus[response.achievementId] = string.Format(achievementSucceededLabel, response.achievementId);
        }
    }

    private void OnRequestFriendsCompleted(AGSRequestFriendIdsResponse response) {
        if (response.IsError ()) {
            friendStatus = string.Format("Error getting friends: {0}", response.error);
        } else {
            if (response.friendIds.Count > 0) {
                friendIds = response.friendIds;
                friendStatus = null;
            } else {
                friendStatus = "Local player has no friends.";
            }
        }
    }

    private void OnRequestAchievementsForFriendCompleted(AGSRequestAchievementsForPlayerResponse response) {
        achievementsForFriend [response.playerId] = new List<string> ();
        if (response.IsError ()) {
            achievementsForFriend [response.playerId].Add(string.Format("Error getting achievements for player: {0}", response.error));
        } else {
            foreach(AGSAchievement achievement in response.achievements) {
                achievementsForFriend [response.playerId].Add(achievement.ToString());
            }
        }
    }

    #endregion

}
