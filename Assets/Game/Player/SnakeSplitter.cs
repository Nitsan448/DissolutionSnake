using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class SnakeSplitter
{
    private SnakeBuilder _snakeBuilder;

    public SnakeSplitter(SnakeBuilder snakeBuilder)
    {
        _snakeBuilder = snakeBuilder;
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
        foreach (SnakeSegment segment in splitSection)
        {
            _snakeBuilder.DestroySegment(segment);

            // TODO: Destroy each segment faster than the previous one
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
    }
}