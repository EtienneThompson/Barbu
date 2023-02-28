using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Round
{
    void goNext(RoundContext input);
    void goPrevious(RoundContext input);
}
