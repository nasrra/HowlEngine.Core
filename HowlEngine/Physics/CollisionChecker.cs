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

public class AABBPhysicSystem{
    private StructPool<PhysicsBodyAABB> physicsBodies; 
    private StructPool<RectangleColliderStruct> staticBodies;    

    public AABBPhysicSystem(int staticColliderAmount, int physicsBodyAmount){
        staticBodies = new StructPool<RectangleColliderStruct>(staticColliderAmount);
        physicsBodies = new StructPool<PhysicsBodyAABB>(physicsBodyAmount);
    }

    public Token AddPyhsicsBody(PhysicsBodyAABB body){
        // allocate.
        Token token = physicsBodies.Allocate();
        if(token.Valid == false){
            return token;
        }

        physicsBodies.TryGetData(ref token).Data = body;
        return token;
    }

    public Token AddStaticBody(RectangleColliderStruct body){
        // allocate.
        Token token = staticBodies.Allocate();
        if(token.Valid == false){
            return token;
        }

        // set data.
        staticBodies.TryGetData(ref token).Data = body;
        return token;
    }

    public void RemoveStaticBody(int index){
        staticBodies.Free(index);
    }

    public void RemovePhysicsBody(int index){
        physicsBodies.Free(index);
    }

    public void RemoveLastStaticBody(){
        staticBodies.Free(staticBodies.Count - 1);
    }

    public void RemoveLastPhysicsBody(){
        physicsBodies.Free(physicsBodies.Count - 1);
    }

    // check distance then start the AABB Collision check.
    public void CheckCollisions(){
        for(int i = 0; i < staticBodies.Capacity; i++){
            // pass the slot if it is not in use.
            if(staticBodies.IsSlotActive(i) == false){
                continue;
            }
            ref RectangleColliderStruct r1 = ref staticBodies.GetData(i);
            for(int j = 0; j < staticBodies.Capacity; j++){
                // pass the slot if it is not in use or is the current collider.
                if(j==1 || staticBodies.IsSlotActive(j) == false){
                    continue;
                }
                ref RectangleColliderStruct r2 = ref staticBodies.GetData(j);

                // Update the CurrentFrameCollisions to account for said collision.
                AABB(ref r1, ref r2);
            }
        }
    }

    public void FixedUpdate(GameTime gameTime){
        for(int i = 0; i < physicsBodies.Capacity; i++){
            // pass the slot ifit is not in use.
            if(physicsBodies.IsSlotActive(i) == false){
                continue;
            }
            ref PhysicsBodyAABB pA = ref physicsBodies.GetData(i);
            float?[] timeOfImpact = null;
            for(int j = 0; j < physicsBodies.Capacity; j++){
                // pass if it is the current collider or the slot ifit is not in use.
                if(j==i || physicsBodies.IsSlotActive(j) == false){
                    continue;
                }
                ref PhysicsBodyAABB pB = ref physicsBodies.GetData(j);
                float?[] newTimeOfImpact = SweptAABB(ref pA, ref pB);
                if(newTimeOfImpact != null){
                    if(timeOfImpact != null){
                        if(newTimeOfImpact[0] != null){
                            if(timeOfImpact[0] != null){
                                float? x1 = timeOfImpact[0];
                                float? x2 = newTimeOfImpact[0];
                                timeOfImpact[0] = x1<x2? x1 : x2;
                            }
                            else{
                                timeOfImpact[0] = newTimeOfImpact[0];
                            }

                        }
                        if(newTimeOfImpact[1] != null){
                            if(timeOfImpact[1] != null){
                                float? y1 = timeOfImpact[1];
                                float? y2 = newTimeOfImpact[1];
                                timeOfImpact[1] = y1<y2? y1 : y2;
                            }
                            else{
                                timeOfImpact[1] = newTimeOfImpact[1];
                            }
                        }
                    }
                    else{
                        timeOfImpact = newTimeOfImpact;
                    }
                }
            }
            if(timeOfImpact != null){
                // Console.WriteLine("collision");
                pA.Position += new Vector2(
                    (float)(timeOfImpact[0] != null? pA.Velocity.X * timeOfImpact[0] : pA.Velocity.X),
                    (float)(timeOfImpact[1] != null? pA.Velocity.Y * timeOfImpact[1] : pA.Velocity.Y)
                );
            }
            else{
                pA.Position += pA.Velocity;
            }
        }
    }


    public void SetStaticBodyPosition(ref Token token, int x, int y){
        RefView<RectangleColliderStruct> rf = staticBodies.TryGetData(ref token);
        if(rf.Valid== false){
            return;
        }
        ref RectangleColliderStruct box = ref rf.Data;
        box.X = x;
        box.Y = y;
    }

    public void SetPhysicsBodyVelocity(ref Token token, Vector2 velocity){
        RefView<PhysicsBodyAABB> rf = GetPhysicsBody(ref token);
        if(rf.Valid==false){
            return;
        }
        ref PhysicsBodyAABB p = ref rf.Data;
        p.Velocity = velocity;
    }

    public void DrawAllOutlines(SpriteBatch spriteBatch, Color color, int thickness){
        for(int i = 0; i < staticBodies.Capacity; i++){
            if(staticBodies.IsSlotActive(i)==false){
                return;
            }
            DrawOutline(ref staticBodies.GetData(i), spriteBatch, color, thickness);
        }
        for(int i = 0; i < physicsBodies.Capacity; i++){
            if(physicsBodies.IsSlotActive(i)==false){
                return;
            }
            ref PhysicsBodyAABB p = ref physicsBodies.GetData(i);
            ref RectangleColliderStruct collider = ref p.Collider;
            DrawOutline(ref collider, spriteBatch, color, thickness);
        }
    }

    public void DrawOutline(ref Token[] tokens, SpriteBatch spriteBatch, Color color, int thickness){
        for(int i = 0; i < tokens.Length; i++){
            DrawOutline(ref tokens[i], spriteBatch, color, thickness);
        }
    }

    public void DrawOutline(ref Token token, SpriteBatch spriteBatch, Color color, int thickness){
        RefView<RectangleColliderStruct> rf = staticBodies.TryGetData(ref token);
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

    public RefView<PhysicsBodyAABB> GetPhysicsBody(ref Token token){
        return physicsBodies.TryGetData(ref token);
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
    public bool AABB(ref RectangleColliderStruct boxA, ref RectangleColliderStruct boxB){
        return 
            boxA.Left < boxB.Right &&
            boxA.Right > boxB.Left &&
            boxA.Top < boxB.Bottom &&
            boxA.Bottom > boxB.Top;
    }

    //===========================================================================================
    // SweptAABB:
    // First pass:
    //  Check to see if there will be a collision on any of the axis.
    //  Retrieving a time value for when that colllision will occcur.
    //
    // Second Pass:
    //  Verify that the collision is not just on one axes but both.
    //  Performing an AABB collision check for the applied movement.
    //  If there has been a collision, the movement is effected by the calculated time of impact.
    //  If not, then the movement continues as normal.
    //===========================================================================================

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pA"></param>
    /// <param name="pB"></param>
    /// <returns></returns>
    public static float?[] SweptAABB(ref PhysicsBodyAABB pA, ref PhysicsBodyAABB pB){
        // get the relative velocity between both physics bodies.
        Vector2 relVel = pA.Velocity - pB.Velocity;

        // How far the body has to travel on the axes to start touching the other body.
        float xEntry, yEntry;

        // How far the body has travel on the axes to completely pass beyond the other body.
        float xExit, yExit;

        // if a body is moving the x-direction.
        // calculate the position to be considered a 'hit' and 'pass through'
        if (relVel.X > 0){
            xEntry = pB.Left - pA.Right;
            xExit = pB.Right - pA.Left;
        }
        else{
            xEntry = pB.Right - pA.Left;
            xExit = pB.Left - pA.Right;
        }

        // if a body is moving the x-direction.
        // calculate the position to be considered a 'hit' and 'pass through'
        if (relVel.Y > 0){
            // Moving downward
            yEntry = pB.Top - pA.Bottom;
            yExit = pB.Bottom - pA.Top;
        }
        else{
            // Moving upward
            yEntry = pB.Bottom - pA.Top;
            yExit = pB.Top - pA.Bottom;
        }

        // determine timings.
        // distance / velocity = time.
        float xEntryTime = (relVel.X == 0) ? float.NegativeInfinity : xEntry / relVel.X;
        float xExitTime = (relVel.X == 0) ? float.PositiveInfinity : xExit / relVel.X;
        float yEntryTime = (relVel.Y == 0) ? float.NegativeInfinity : yEntry / relVel.Y;
        float yExitTime = (relVel.Y == 0) ? float.PositiveInfinity : yExit / relVel.Y;

        // get the latest possible time of entry to ensure the body can move the full amount.
        float entryTime = Math.Max(xEntryTime, yEntryTime);
        // get the earliest possible time of exit to ensure the body can move the full amount.
        float exitTime = Math.Min(xExitTime, yExitTime);

        // Console.WriteLine($"{entryTime > exitTime} {entryTime < 0f} {entryTime > 1f}");
        if (entryTime > exitTime || entryTime < 0f || entryTime > 1f)
            return null; // no collision within this frame
        
        // calculate the effected movement from the supposed impact.
        Vector2 pointOfImpact = pA.Position + pA.Velocity * entryTime;
        RectangleColliderStruct movementA = new RectangleColliderStruct((int)pointOfImpact.X, (int)pointOfImpact.Y, pA.Width, pA.Height);
        
        // Check if there is an AABB overlap at the point of impact.
        // If there is, it is a valid change in movement.
        bool validOverlap = false;
        float?[] timeOfImpact = new float?[2];
        if (xEntryTime > yEntryTime){
            if(movementA.Bottom > pB.Top && movementA.Top < pB.Bottom){
                timeOfImpact[0] = entryTime;
                timeOfImpact[1] = null;
                validOverlap = true;
            }
        }
        else{
            if(movementA.Right > pB.Left && movementA.Left < pB.Right){
                timeOfImpact[0] = null;
                timeOfImpact[1] = entryTime;
                validOverlap = true;
            }
        }

        if (!validOverlap)
            return null;

        return timeOfImpact;
    }

    public bool SweptAABB(ref PhysicsBodyAABB pA, ref RectangleColliderStruct bB){
        return false;
    }
}
