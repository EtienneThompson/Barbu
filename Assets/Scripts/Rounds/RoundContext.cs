using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundContext
{
    private Round current;

    public RoundContext()
    {
    }

    public void Next()
    {
        current.goNext(this);
    }

    public void Previous()
    {
        current.goPrevious(this);
    }

    public void setState(Round current)
    {
        this.current = current;
    }

    public Round getCurrentRound()
    {
        return this.current;
    }
}
