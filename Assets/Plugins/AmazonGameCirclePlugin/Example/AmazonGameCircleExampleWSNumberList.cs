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
/// Amazon GameCircle example Whispersync number list.
/// </summary>
public class AmazonGameCircleExampleWSNumberList {
    // Some helper enums so there can be a single function
    // for displaying a whispersync number, instead of one per number of behaviors times available types.
    #region local enums
    // These are the types of number lists available.
    public enum AvailableListType {
        HighNumber,
        LowNumber,
        LatestNumber,
    };
    #endregion
    
    #region local variables
    // the type of this list
    private readonly AvailableListType listType = AvailableListType.HighNumber;
    // the Whispersync list information
    private AGSSyncableNumberList syncableNumberList = null;
    // Accessors to the elements in the list.
    private AGSSyncableNumberElement[] syncableNumberElements = null;
    // a local copy of the data in the list.
    // It's important to cache this data locally, as it is
    // slow to access data out of a native (iOS / Android) plugin.
    private AmazonGameCircleExampleWSNumberListElementCache [] syncableNumberElementsCache = null;
        
    // Instead of calling "isSet" every time the menu is redrawn,
    // it is faster and more efficient to store that value locally.
    private bool isSet = false;
    
    // these are sample values to be added to the list.
    // incremented each time to test lists with multiple values.
    private int intVal = 0;
    private long longVal = 0;
    private double doubleVal = 0.0;
    
    // this is the maximum number of values the list will hold
    // Using a nullable so it can be initialized when the list is initialized.
    private int? maxSize = null;
    
    // foldouts keep menus clean
    private bool foldout = false;
    #endregion
    
    #region Local const strings
    // This label is displayed if this class is not initialized.
    private const string notInitializedLabel = "Syncable number list not yet initialized";
    // This is the label for the button that refreshes the number elements list.
    private const string refreshSyncableNumberElementsButtonLabel = "Refresh List";
    // This label is displayed if the list of elements in this list is empty.
    private const string emptyListLabel = "List is empty";
    // This is the label for the button that adds entries to the list.
    private const string addValuesButtonLabel = "Add values";
    // these are values used in the metadata dictionary, simple values to test behavior.
    private const string metadataKey = "key";
    private const string metadataValue = "value";
    // This is the label for the max size slider
    private const string maxSizeLabel = "Max Size {0}";
    // This is the label for the button that updates the max size
    private const string updateMaxSizeButtonLabel = "Update Max Size";
    // This is the label for checking if the list is set.
    private const string isListSetLabel = "Has list been set yet? {0}";
    #endregion
    
    #region local const values
    // Every time a value is added, it is incremented by one of these values.
    private const int intIncrement = 1;
    private const long longIncrement = -5;
    private const double doubleIncrement = 0.1;
    // Instead of storing a string value locally, and incrementing it by going
    // back and forth to string, the int value is just muliplied by this multiplier.
    private const int stringMultiplier = 2;
    
    // The minimum and maximum values for the max size list slider
    private const int minMaxSize = 3;
    private const int maxMaxSize = 8;
    
    // This is a sample metadata dictionary, with a single key value pair.
    private readonly Dictionary<string,string> defaultMetadataDictionary = new Dictionary<string, string>() { { metadataKey, metadataValue } };
    #endregion
    
    #region constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleWSNumberList"/> class.
    /// </summary>
    /// <param name='availableListType'>
    /// Available list type.
    /// </param>
    public AmazonGameCircleExampleWSNumberList(AvailableListType availableListType) {
        listType = availableListType;   
    }
    #endregion
    
    #region public functions
    /// <summary>
    /// Draws an interface to interact with a number list.
    /// </summary>
    /// <param name='dataMap'>
    /// Data map.
    /// </param>
    public void DrawGUI(AGSGameDataMap dataMap) {
        // make sure necessary variables are available before drawing anything.
        if(null == dataMap) {
            AmazonGUIHelpers.CenteredLabel(notInitializedLabel);
            return;
        }
        
        // make sure the number list is initialized before using it.
        if(null == syncableNumberList) {
            InitSyncableNumberList(dataMap);
        }
        
        if(null == syncableNumberList) {
            AmazonGUIHelpers.CenteredLabel(notInitializedLabel);
            return;   
        }
        // putting a box around the list helps visually separate the lists in the menu.
        GUILayout.BeginVertical(GUI.skin.box);
        
        // foldouts help keep menus clean.
        foldout = AmazonGUIHelpers.FoldoutWithLabel(foldout,ListName());
        
        if(foldout) {
            // this button refreshes the list, and retrieves elements in it.
            if(GUILayout.Button(refreshSyncableNumberElementsButtonLabel)) {
                RefreshList();   
            }
            
            // This label displays if this list has been set.
            AmazonGUIHelpers.CenteredLabel(string.Format(isListSetLabel,isSet));
            
            // This label displays the max size (if the size has been retrieved).
            if(maxSize.HasValue) {
                maxSize = (int)AmazonGUIHelpers.DisplayCenteredSlider((float)maxSize.Value,minMaxSize,maxMaxSize,maxSizeLabel);
                if(GUILayout.Button(updateMaxSizeButtonLabel)) {
                    syncableNumberList.SetMaxSize(maxSize.Value);
                }
            }
            
            // if the list is empty, display a message
            if(null == syncableNumberElementsCache || 0 == syncableNumberElementsCache.Length) {
                AmazonGUIHelpers.CenteredLabel(emptyListLabel);
            }
            // display each element in the list.
            else {
                foreach(AmazonGameCircleExampleWSNumberListElementCache cachedElement in syncableNumberElementsCache) {
                    cachedElement.DrawElement();   
                }
            }
            
            // this button adds some values to the list, increments those values, and refreshes the list.
            // automatically incrementing values every time they are added allows this menu
            // to remain simple, without value selection logic.
            if(GUILayout.Button(addValuesButtonLabel)) {
                AddValuesToList();   
                // increment between adding values to make sure the metadata entries are different.
                IncrementValues();
                AddValuesToListWithMetadata();   
                IncrementValues();
                RefreshList();
            }
        }
        GUILayout.EndVertical();
    }
    #endregion
    
    #region private functions    
    /// <summary>
    /// Initializes the syncable number list.
    /// </summary>
    /// <param name='dataMap'>
    /// Data map.
    /// </param>
    void InitSyncableNumberList(AGSGameDataMap dataMap) {
        // initialize the list based on what type of list it is.
        switch(listType) {
        case AvailableListType.HighNumber:
            syncableNumberList = dataMap.GetHighNumberList(ListName());
            break;
        case AvailableListType.LatestNumber:
            syncableNumberList = dataMap.GetLatestNumberList(ListName());
            break;
        case AvailableListType.LowNumber:
            syncableNumberList = dataMap.GetLowNumberList(ListName());
            break;
        }
        // cache the size and if the list has been set.
        maxSize = syncableNumberList.GetMaxSize();
        isSet = syncableNumberList.IsSet();
    }
    
    /// <summary>
    /// Refreshs the list of elements.
    /// </summary>
    void RefreshList() {
        if(null == syncableNumberList) {
            return;
        }
        
        // Check if the max size has changed, or the list has become unset.
        maxSize = syncableNumberList.GetMaxSize();
        isSet = syncableNumberList.IsSet();
        
        syncableNumberElements = syncableNumberList.GetValues();
        syncableNumberElementsCache = new AmazonGameCircleExampleWSNumberListElementCache[syncableNumberElements.Length];
        
        // caching the data in the list locally allows for fast access to this information.
        for(int listIndex = 0; listIndex < syncableNumberElements.Length; listIndex++) {
            syncableNumberElementsCache[listIndex] = 
                new AmazonGameCircleExampleWSNumberListElementCache(syncableNumberElements[listIndex].AsInt(),
                                                                    syncableNumberElements[listIndex].AsLong(),
                                                                    syncableNumberElements[listIndex].AsDouble(),
                                                                    syncableNumberElements[listIndex].AsString(),
                                                                    syncableNumberElements[listIndex].GetMetadata());                
        }
        
    }
    
    /// <summary>
    /// Adds a value of each accepted type to the list, without a metadata dictionary.
    /// </summary>
    void AddValuesToList() {
        // add as an int, long, and double.
        syncableNumberList.Add(intVal);
        syncableNumberList.Add(longVal);
        syncableNumberList.Add(doubleVal);
        // Add a value as a string. Instead of going back and forth from
        // number to string, this is just the int multiplied by a multiplier.
        syncableNumberList.Add((intVal*stringMultiplier).ToString());
    }
    
    /// <summary>
    /// Adds a value of each accepted type to the list, with a metadata dictionary.
    /// </summary>
    void AddValuesToListWithMetadata() {
        // add as an int, long, and double.
        syncableNumberList.Add(intVal,defaultMetadataDictionary);
        syncableNumberList.Add(longVal,defaultMetadataDictionary);
        syncableNumberList.Add(doubleVal,defaultMetadataDictionary);
        // Add a value as a string. Instead of going back and forth from
        // number to string, this is just the int multiplied by a multiplier.
        syncableNumberList.Add((intVal*stringMultiplier).ToString(),defaultMetadataDictionary);
    }
    
    /// <summary>
    /// Increments the values to be added to the list.
    /// This is an easy way to test adding more values of different ammounts to the list.
    /// </summary>
    void IncrementValues() {
        intVal += intIncrement;   
        longVal += longIncrement;
        doubleVal += doubleIncrement;
    }
    
    /// <summary>
    /// The name of the list
    /// </summary>
    /// <returns>
    /// The name.
    /// </returns>
    string ListName() {
        return listType.ToString();   
    }
    #endregion
}
