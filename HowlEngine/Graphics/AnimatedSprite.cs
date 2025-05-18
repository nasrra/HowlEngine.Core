using System;
using System.Collections.Generic;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public class AnimatedSprite : Sprite{
    private int _currentFame = 0;
    private TimeSpan _elapsedTime = new TimeSpan();
    private Animation _animation;

    public Animation Animation{
        get => _animation;
        set {
            _animation = value;
            // set the currentr region to be thef irst frame of the animation.
            TextureRegion = _animation.Frames[0];
        }
    }

    /// <summary>
    /// Creates a new AnimatedSprite.
    /// </summary>
    public AnimatedSprite(){}

    /// <summary>
    /// Creates a new AnimatedSprite.
    /// </summary>
    /// <param name="animation">The animation to start playing.</param>
    public AnimatedSprite(Animation animation){
        this.Animation = animation;
    }

    /// <summary>
    /// Updates this animated sprite.
    /// </summary>
    /// <param name="gameTime">The current game timing values provided by monogame.</param>
    public void Update(GameTime gameTime){
        _elapsedTime += gameTime.ElapsedGameTime;
        if(_elapsedTime >= _animation.Interval){
            _elapsedTime -= _animation.Interval;
            _currentFame = _currentFame >= Animation.Frames.Count-1? 0 : _currentFame +=1;
            TextureRegion = _animation.Frames[_currentFame];
        }
    }
}