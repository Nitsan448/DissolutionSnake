using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SnakeSplitter
{
    private SnakeBuilder _snakeBuilder;

    //TODO: rename
    private float _snakeDissolutionStartingSpeed;

    public SnakeSplitter(SnakeBuilder snakeBuilder, float snakeDissolutionStartingSpeed)
    {
        _snakeBuilder = snakeBuilder;
        _snakeDissolutionStartingSpeed = snakeDissolutionStartingSpeed;
    }

    public void SplitSnake(LinkedListNode<SnakeSegment> nodeToStartSplittingFrom)
    {
        LinkedList<SnakeSegment> splitSection = SplitSection(nodeToStartSplittingFrom);
        DissoulteSplitSection(splitSection).Forget();
        _snakeBuilder.SetNewMiddleNode();
    }

    //TODO: rename
    private LinkedList<SnakeSegment> SplitSection(LinkedListNode<SnakeSegment> nodeToStartSplittingFrom)
    {
        // TODO: Refactor
        LinkedListNode<SnakeSegment> current = nodeToStartSplittingFrom;
        LinkedList<SnakeSegment> splitSection = new LinkedList<SnakeSegment>();

        while (current != null)
        {
            LinkedListNode<SnakeSegment> next = current.Next;
            splitSection.AddLast(current.Value);
            _snakeBuilder.DetachSegment(current);
            current = next;
        }

        return splitSection;
    }

    private async UniTask DissoulteSplitSection(LinkedList<SnakeSegment> splitSection)
    {
        float currentSegmentIndex = 0;
        int numberOfSegmentsToDestroy = splitSection.Count;
        foreach (SnakeSegment segment in splitSection)
        {
            _snakeBuilder.DestroySegment(segment);
            float delayUntilNextSegmentIsDestroyed = _snakeDissolutionStartingSpeed -
                                                     ((currentSegmentIndex / numberOfSegmentsToDestroy) * _snakeDissolutionStartingSpeed);
            await UniTask.Delay(TimeSpan.FromSeconds(delayUntilNextSegmentIsDestroyed), delayTiming: PlayerLoopTiming.FixedUpdate);
            currentSegmentIndex++;
        }
    }
}