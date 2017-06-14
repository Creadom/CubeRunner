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
/// GameCircle example implementation of the GameCircleSocial plugin's leaderboard functionality.
/// </summary>
public class GameCircleSocialLeaderboardsExample {
    
    #region local variables
    // this allows the entire leaderboards menu section to be hidden behind a foldout menu
    bool leaderboardsMenuFoldout = false;
    // This value is used to upload scores to the leaderboard.
    long scoreValue = 0; 
    // This is a list of scores received in the callback after reporting a score.
    IScore[] scores = null;
    // This is a nullable bool value. This allows it to be set to null initially,
    // before score reporting is attempted, and then set to the result of score reporting later.
    bool? scoreReportSuccessful = null;
    #endregion
    
    #region local strings
    // The label for this menu
    private const string leaderboardsMenuLabel = "Leaderboards Menu";  
    // The label for the button that opens the achievement overlay
    private const string leaderboardsOverlayLabel = "Leaderboards Overlay";
    // The label for the score value on the slider
    private const string scoreValueLabel = "{0}";
    // The label for the button that reports the score
    private const string reportScoreButtonLabel = "Report Score";
    // This label is displayed when the score report was successful.
    private const string scoreReportSuccessLabel = "Score Report Success";
    // This label is displayed when the score report failed.
    private const string scoreReportFailureLabel = "Score Report Failure";
    // This is the label on the button that retrieves scores
    private const string retrieveScoresButtonLabel = "Retrieve Scores";
    // The label for the score's leaderboard ID
    private const string scoreLeaderboardLabel = "Leaderboard {0}";
    // The label for the score's rank
    private const string scoreRankLabel = "Rank {0}";
    // The label for the score's userID
    private const string scoreUserLabel = "User {0}";    
    
    // The ID for a sample leaderboard. Fill this in with your own leaderboard.
    private const string leaderboardExampleID = "Replace with your own Leaderboard ID";
    #endregion
    
    #region local const variables
    // These are just some minimum and maximum values for the slider to submit a score
    private const long scoreMinValue = 0;
    private const long scoreMaxValue = 10000;
    #endregion
    
    /// <summary>
    /// Draws the leaderboards menu.
    /// </summary>
    public void DrawLeaderboardsMenu() {
        // Hide the leaderboards menu with a foldout.
        leaderboardsMenuFoldout = AmazonGUIHelpers.FoldoutWithLabel(leaderboardsMenuFoldout,leaderboardsMenuLabel);
        if(!leaderboardsMenuFoldout) {
            return;
        }
        
        // This button will open the leaderboards overlay.
        if(GUILayout.Button(leaderboardsOverlayLabel)) {
            Social.ShowLeaderboardUI();   
        }
        
        DisplayScoreReporter();
        DisplayScoreReportResult();
        DisplayScoreRetrieval();
    }
    
    /// <summary>
    /// Displays the interface for reporting a new score.
    /// </summary>
    void DisplayScoreReporter() {
        // This slider will allow for selecting a score to be uploaded.
        scoreValue = (long) AmazonGUIHelpers.DisplayCenteredSlider((float)scoreValue,
                                                                (float)scoreMinValue,
                                                                (float)scoreMaxValue,
                                                                scoreValueLabel);
        // This button will report the score to the leaderboard.
        if(GUILayout.Button(reportScoreButtonLabel)) {
            Social.ReportScore(scoreValue, leaderboardExampleID, ReportScoreCallback);
        }
    }
    
    /// <summary>
    /// Displays the results of reporting a score
    /// </summary>
    void DisplayScoreReportResult() {
        // If a score has not been reported yet,
        // then display an empty line to keep the spacing here correct.
        string reportedScoreResult = string.Empty;
        if(scoreReportSuccessful.HasValue) {
            // Display a message mentioning the status of the score report result.
            reportedScoreResult = scoreReportSuccessful.Value ? scoreReportSuccessLabel : scoreReportFailureLabel;
        }
        AmazonGUIHelpers.CenteredLabel(reportedScoreResult);
    }
        
    /// <summary>
    /// Displays retrieved scores, and a button to retrieve them.
    /// </summary>
    void DisplayScoreRetrieval() {
        if(GUILayout.Button(retrieveScoresButtonLabel)) {
            Social.LoadScores(leaderboardExampleID,RetrieveScoreCallback);   
        }
        
        // If a list of scores has been received, display it.
        if(null != scores) {
            foreach(IScore score in scores) {   
                // Put each score in a box to visually separate them.
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                // Display the leaderboard the score belongs to, and its value on one line.
                AmazonGUIHelpers.CenteredLabel(string.Format(scoreLeaderboardLabel,score.leaderboardID));
                AmazonGUIHelpers.CenteredLabel(score.formattedValue);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                // Display the user and the rank on another line.
                AmazonGUIHelpers.CenteredLabel(string.Format(scoreUserLabel,score.userID));
                AmazonGUIHelpers.CenteredLabel(string.Format(scoreRankLabel,score.rank));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
    }
    
    #region callbacks
    /// <summary>
    /// Callback for handling the result of reporting scores.
    /// </summary>
    /// <param name='success'>
    /// Success.
    /// </param>
    void ReportScoreCallback(bool success) {
        scoreReportSuccessful = success;
    }
    
    /// <summary>
    /// Callback for receiving the list of scores.
    /// </summary>
    /// <param name='scores'>
    /// Scores.
    /// </param>
    void RetrieveScoreCallback(IScore[] scores) {
        this.scores = scores;
    }
    #endregion
}
