using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SnakeSplitter
{
    private SnakeBuilder _snakeBuilder;
    private float _dissolutionStartingDelay;

    public SnakeSplitter(SnakeBuilder snakeBuilder, float dissolutionStartingDelay)
    {
        _snakeBuilder = snakeBuilder;
        _dissolutionStartingDelay = dissolutionStartingDelay;
    }

    public void SplitSnake(LinkedListNode<SnakeSegment> nodeToStartSplitFrom)
    {
        LinkedList<SnakeSegment> detachedSection = DetachSection(nodeToStartSplitFrom);
        DissoluteDetachedSection(detachedSection).Forget();
        _snakeBuilder.SetNewMiddleNode();
    }

    private LinkedList<SnakeSegment> DetachSection(LinkedListNode<SnakeSegment> nodeToStartDetachingFrom)
    {
        LinkedListNode<SnakeSegment> current = nodeToStartDetachingFrom;
        LinkedList<SnakeSegment> detachedSection = new LinkedList<SnakeSegment>();

        while (current != null)
        {
            LinkedListNode<SnakeSegment> next = current.Next;
            detachedSection.AddLast(current.Value);
            current.Value.MakeDetachedNode();
            _snakeBuilder.DetachSegment(current);
            current = next;
        }

        return detachedSection;
    }

    private async UniTask DissoluteDetachedSection(LinkedList<SnakeSegment> splitSection)
    {
        float currentSegmentIndex = 0;
        int numberOfSegmentsToDestroy = splitSection.Count;
        foreach (SnakeSegment segment in splitSection)
        {
            float delayUntilNextSegmentIsDestroyed = _dissolutionStartingDelay * (1 - (currentSegmentIndex / numberOfSegmentsToDestroy));
            await UniTask.Delay(TimeSpan.FromSeconds(delayUntilNextSegmentIsDestroyed));
            _snakeBuilder.DestroySegment(segment);
            currentSegmentIndex++;
        }
    }
}