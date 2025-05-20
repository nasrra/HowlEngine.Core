using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HowlEngine.Collections;
using HowlEngine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    
    StructPool<RectangleColliderStruct> aabbStructs = new StructPool<RectangleColliderStruct>(10);
    
    // List<RectangleColliderStruct> aabbStructs = new List<RectangleColliderStruct>();
    List<RectangleColliderClass> aabbClass = new List<RectangleColliderClass>();

    public void AddAABBClass(RectangleColliderClass r){
        aabbClass.Add(r);
    }

    public Token AddAABBStruct(RectangleColliderStruct r){
        // allocate.
        Token token = aabbStructs.Allocate();
        if(token.Valid == false){
            return token;
        }

        // set data.
        aabbStructs.TryGetData(ref token).Data = r;
        return token;
    }

    public void RemoveAABBStruct(int index){
        aabbStructs.Free(index);
    }

    public void RemoveAABBStructLast(){
        aabbStructs.Free(aabbStructs.Count - 1);
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
        for(int i = 0; i < aabbStructs.Capacity; i++){
            // pass the slot if it is not in use.
            if(aabbStructs.IsSlotActive(i) == false){
                continue;
            }
            ref RectangleColliderStruct r1 = ref aabbStructs.GetData(i);
            for(int j = 0; j < aabbStructs.Capacity; j++){
                // pass the slot if it is not in use or is the current collider.
                if(j==1 || aabbStructs.IsSlotActive(j) == false){
                    continue;
                }
                ref RectangleColliderStruct r2 = ref aabbStructs.GetData(j);
                AABBStruct(ref r1, ref r2);
            }
        }
    }

    public void UpdatePosition(ref Token token, int x, int y){
        RefView<RectangleColliderStruct> rf = aabbStructs.TryGetData(ref token);
        if(rf.Valid== false){
            return;
        }
        ref RectangleColliderStruct box = ref rf.Data;
        box.X = x;
        box.Y = y;
    }

    public void DrawAllOutlines(SpriteBatch spriteBatch, Color color, int thickness){
        for(int i = 0; i < aabbStructs.Capacity; i++){
            if(aabbStructs.IsSlotActive(i)==false){
                return;
            }
            DrawOutline(ref aabbStructs.GetData(i), spriteBatch, color, thickness);
        }
    }

    public void DrawOutline(ref Token[] tokens, SpriteBatch spriteBatch, Color color, int thickness){
        for(int i = 0; i < tokens.Length; i++){
            DrawOutline(ref tokens[i], spriteBatch, color, thickness);
        }
    }

    public void DrawOutline(ref Token token, SpriteBatch spriteBatch, Color color, int thickness){
        RefView<RectangleColliderStruct> rf = aabbStructs.TryGetData(ref token);
        if(rf.Valid == false){
            return;
        }
        DrawOutline(ref rf.Data,spriteBatch,color, thickness);
    }

    private void DrawOutline(ref RectangleColliderStruct box, SpriteBatch spriteBatch, Color color, int thickness){
        // Top.
        spriteBatch.Draw(HowlApp.Instance.DebugTexture, new Rectangle(box.X, box.Y, box.Width, thickness), color);
        // Left.
        spriteBatch.Draw(HowlApp.Instance.DebugTexture, new Rectangle(box.X, box.Y, thickness, box.Height), color);
        // Right.
        spriteBatch.Draw(HowlApp.Instance.DebugTexture, new Rectangle(box.Right-thickness, box.Y, thickness, box.Height), color);
        // Bot.
        spriteBatch.Draw(HowlApp.Instance.DebugTexture, new Rectangle(box.X, box.Bottom-thickness, box.Width, thickness), color);
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
