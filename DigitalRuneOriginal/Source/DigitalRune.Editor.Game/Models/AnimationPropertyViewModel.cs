// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using MinimalRune.Animation.Character;
using MinimalRune.Windows;
using MinimalRune.Windows.Framework;


namespace MinimalRune.Editor.Models
{
    /// <summary>
    /// Describes a single animation value in a property grid.
    /// </summary>
    internal class AnimationPropertyViewModel : ObservableObject
    {
        

        

        private readonly ModelDocument _document;



        

        

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;


        public SkeletonKeyFrameAnimation Animation
        {
            get { return _animation; }
            set { SetProperty(ref _animation, value); }
        }
        private SkeletonKeyFrameAnimation _animation;


        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (SetProperty(ref _isPlaying, value))
                {
                    if (value)
                        _document.PlayAnimation(Name);
                    else
                        _document.StopAnimation();
                }
            }
        }
        private bool _isPlaying;


        public DelegateCommand ToggleIsPlayingCommand { get; }



        

        

        public AnimationPropertyViewModel(ModelDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _document = document;
            ToggleIsPlayingCommand = new DelegateCommand(OnToggleIsPlaying);
        }



        

        

        private void OnToggleIsPlaying()
        {
            IsPlaying = !IsPlaying;
        }

    }
}
