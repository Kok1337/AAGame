using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    private const float MaxAlpha = 1f;
    private const float MinAlpha = .45f;
    private const float FadeStep = .005f;
    private const float UnfadeStep = .005f;

    private FadeState _fadeState;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _fadeState = FadeState.Stable;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (_fadeState != FadeState.Stable)
        {
            switch (_fadeState)
            {
                case FadeState.Fade:
                    Fade();
                    break;
                case FadeState.Unfade:
                    Unfade();
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _fadeState = FadeState.Fade;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _fadeState = FadeState.Unfade;
    }

    private void Fade()
    {
        Color color = _spriteRenderer.material.color;
        color.a -= FadeStep;
        if (color.a <= MinAlpha)
        {
            color.a = MinAlpha;
            _fadeState = FadeState.Stable;
        }
        _spriteRenderer.material.color = color;
    }

    private void Unfade()
    {
        Color color = _spriteRenderer.material.color;
        color.a += UnfadeStep;
        if (color.a >= MaxAlpha)
        {
            color.a = MaxAlpha;
            _fadeState = FadeState.Stable;
        }
        _spriteRenderer.material.color = color;
    }

    private enum FadeState
    {
        Stable,
        Fade,
        Unfade
    }
}
