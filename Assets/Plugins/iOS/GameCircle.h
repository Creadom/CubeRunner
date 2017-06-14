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

#import <Foundation/Foundation.h>

/**
 * \class GameCircle
 * The top level client for all Amazon Games functionality.
 * This is a stripped down version of the header to minimize
 * the number of files in the Plugins\iOS folder for Unity.
 */
@interface GameCircle : NSObject

/**
 * Handles behavior for when the game's application delegate class receives a URL call
 * from Login With Amazon's authentication page.  Call this to ensure the GameCircle client 
 * is able to sign the customer in successfully.
 *
 * \arg url The custom URL call received by application delegate
 * \arg sourceApplication The application making this custom URL call
 */
+ (BOOL) handleOpenURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication;


@end
