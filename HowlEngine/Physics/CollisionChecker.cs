using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace HowlEngine.Physics;

//==========================================================================================
// NOTES:
//
// Broad Phase: A quick, simple check to rule out objects that definitely are not colliding.
//  - do a proximity check of double that of the bounding box width as a length.
//
// Narrow Phase: A more precise check only performed on objects that passed the broad phase.
// - AABB or SAT collision checking.
//
//==========================================================================================

public class CollisionChecker{
    

    List<RectangleColliderStruct> aabbStructs = new List<RectangleColliderStruct>();
    List<RectangleColliderClass> aabbClass = new List<RectangleColliderClass>();

    public void AddAABBClass(RectangleColliderClass r){
        aabbClass.Add(r);
    }

    public void AddAABBStruct(RectangleColliderStruct r){
        aabbStructs.Add(r);
    }

    public void CheckCollisionsClass(){
        for(int i = 0; i < aabbClass.Count; i++){
            RectangleColliderClass r1 = aabbClass[i];
            for(int j = 0; j < aabbClass.Count; j++){
                if(j==i){
                    continue;
                }
                AABBClass(r1, aabbClass[j]);
            }
        }
    }

    // check distance then start the AABB Collision check.
    public void CheckCollisionsStruct(){
        Span<RectangleColliderStruct> span = CollectionsMarshal.AsSpan(aabbStructs);
        for(int i = 0; i < span.Length; i++){
            ref RectangleColliderStruct r1 = ref span[i];
            for(int j = 0; j < span.Length; j++){
                if(j==i){
                    continue;
                }
                ref RectangleColliderStruct r2 = ref span[j];
                AABBStruct(ref r1, ref span[j]);
            }
        }
    }


    //========================================================================================
    // When using AABB collision detection, there are four conditions that must be true
    // in order to say "a collision has occured". Given Bounding Box [A] and Bounding Box [B]
    // 
    //  1.  The left edge (x-position) of [A] must be less than the right edge
    //      (x-position + width) of [B].
    //
    //  2.  The right edge (x-position+width) of [A] must be less than the left edge
    //      (x-position) of [B].
    //
    //  3.  The top edge (y-position) of [A] must be less than the bottom edge
    //      (y-position + height) of [B].
    //
    //  4.  The bottom edge (y-position+height) of [A] must be less than the top edge
    //      (y-position) of [B].
    //
    //========================================================================================

    /// <summary>
    /// Checks for a collision between two rectangular structures using Axis-Aligned
    /// Bounding Box collision detection.
    /// </summary>
    /// <param name="boxA">The bounding box for the first structure.</param>
    /// <param name="boxB">The bounding box for the second structure.</param>
    /// <returns>true, if the two boxes are colliding; otherwise false.</returns>
    public bool AABBClass(RectangleColliderClass boxA, RectangleColliderClass boxB){
        return 
            boxA.Left < boxB.Right &&
            boxA.Right > boxB.Left &&
            boxA.Top < boxB.Bottom &&
            boxA.Bottom > boxB.Top;
    }

    public bool AABBStruct(ref RectangleColliderStruct boxA, ref RectangleColliderStruct boxB){
        return 
            boxA.Left < boxB.Right &&
            boxA.Right > boxB.Left &&
            boxA.Top < boxB.Bottom &&
            boxA.Bottom > boxB.Top;
    }

}
