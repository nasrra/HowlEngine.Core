using System;
using System.Collections.Generic;

namespace HowlEngine.Graphics;

public struct Animation{
    /// <summary>
    /// The texture regions that make up the frames of this animation.
    /// The order of the regions stored is the order they are displayed in.
    /// </summary>
    public TextureRegion[] Frames {get;set;}

    /// <summary>
    /// The amount of time to delay between each frame, in milliseconds, before moving to the next frame.
    /// </summary>
    public TimeSpan Interval {get;set;}

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    public Animation(int frames){
        Frames = new TextureRegion[frames];
        Interval = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    /// <param name="frames">An ordered collection of the frames for this animation.</param>
    /// <param name="interval">The amount of time to delay between each frame, in milliseconds, before moving to the next frame.</param>
    public Animation(TextureRegion[] frames, TimeSpan interval){
        Frames = frames;
        Interval = interval;
    }
}