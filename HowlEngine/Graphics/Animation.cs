using System;
using System.Collections.Generic;

namespace HowlEngine.Graphics;

public class Animation{
    /// <summary>
    /// The texture regions that make up the frames of this animation.
    /// The order of the regions stored is the order they are displayed in.
    /// </summary>
    public List<TextureRegion> frames {get;set;}

    /// <summary>
    /// The amount of time to delay between each frame, in milliseconds, before moving to the next frame.
    /// </summary>
    public TimeSpan interval {get;set;}

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    public Animation(){
        frames = new List<TextureRegion>();
        interval = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    /// <param name="frames">An ordered collection of the frames for this animation.</param>
    /// <param name="interval">The amount of time to delay between each frame, in milliseconds, before moving to the next frame.</param>
    public Animation(List<TextureRegion> frames, TimeSpan interval){
        this.frames = frames;
        this.interval = interval;
    }
}