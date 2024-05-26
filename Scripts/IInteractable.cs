using System;
using System.Collections.Generic;

using Godot;

using JamEnums;

// Interface that an interactable object should implement if it can be interacted with.
// The necessary information / state to act should be stored on the class as only the
// actor gets passed in when triggering action.
interface IInteractable<T> {
    void Interact(T withActor);

    // return true/false whether or not this interactable works with a specific actor type
    bool InteractsWith(ActorType actorType);

    // What's the quip to interact with this object
    string ActionSummary();

    static IInteractable<U> FromArea2D<U>(Area2D area) {
        return FromArea2D<U>(area, null);
    }
    
    static IInteractable<U> FromArea2D<U>(Area2D area, ActorType? checkType) {
        var node = area.GetParent();
        // TODO: does the <U> refinement ever work at runtime in csharp?
        if (node is IInteractable<U>) {
            var ii = (IInteractable<U>)node;
            if (checkType != null && !ii.InteractsWith((ActorType)checkType)) {
                return null;
            }
            return ii;
        }
        return null;
    }
}