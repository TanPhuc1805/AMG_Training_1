using DG.Tweening;
using UnityEngine;

public class BouncingBallDotween : MonoBehaviour
{
    public float bouncingStrength = 0.8f; // Hệ số nảy (0.8 = mất 20% lực sau mỗi lần nảy)
    public float force = 5f; // Lực ném ban đầu
    public float angle = 45f; // Góc ném ban đầu (độ)
    public float gravity = 9.8f; // Trọng lực
    public float floorY = 0f; // Mặt đất (độ cao mà bóng sẽ nảy lên)
    public float stopThreshold = 0.1f; // Ngưỡng dừng nảy

    private Vector2 velocity; // Vận tốc ban đầu

    void Start()
    {
        float rad = angle * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(rad) * force, Mathf.Sin(rad) * force);

        FallDownFirst();
    }

    void FallDownFirst()
    {
        float startY = transform.position.y;
        float timeToFall = Mathf.Sqrt(2 * (startY - floorY) / gravity); // Công thức đúng

        float nextX = transform.position.x + velocity.x * timeToFall;
        float newVelocityY = velocity.y - gravity * timeToFall; // Cập nhật vận tốc sau rơi

        Debug.Log($"Initial Fall Time: {timeToFall:F2}");
        Debug.Log($"Initial X Position: {nextX:F2}");
        Debug.Log($"Velocity Y After Fall: {newVelocityY:F2}");

        Sequence fallSequence = DOTween.Sequence();

        // Rơi xuống đất
        fallSequence.Append(transform.DOMoveY(floorY, timeToFall).SetEase(Ease.InQuad));

        // Di chuyển ngang trong khi rơi
        fallSequence.Join(transform.DOMoveX(nextX, timeToFall).SetEase(Ease.Linear));

        // Khi chạm đất, bắt đầu nảy
        fallSequence.OnComplete(() =>
        {
            SimulateBounce(nextX, floorY, new Vector2(velocity.x* bouncingStrength, -newVelocityY * bouncingStrength));
        });
    }

    void SimulateBounce(float startX, float startY, Vector2 currentVelocity)
    {
        if (Mathf.Abs(currentVelocity.y) < stopThreshold||Mathf.Abs(currentVelocity.x) < stopThreshold) return; // Dừng khi lực quá nhỏ

        float peakHeight = startY + (currentVelocity.y * currentVelocity.y) / (2 * gravity); // Độ cao cực đại
        float timeToFall = Mathf.Sqrt(2 * (peakHeight - floorY) / gravity); // Thời gian rơi từ đỉnh xuống
        float totalTime = 2 * timeToFall; // Tổng thời gian lên và xuống
        float nextXPeak = startX + currentVelocity.x * timeToFall; // Tọa độ X tiếp theo
        float nextX = startX + currentVelocity.x * totalTime; // Tọa độ X tiếp theo

        Debug.Log($"Bounce Peak Height: {peakHeight:F2}");
        Debug.Log($"Bounce Time: {totalTime:F2}");
        Debug.Log($"Next X Position: {nextX:F2}");

        Sequence bounceSequence = DOTween.Sequence();

        // Bay lên
        bounceSequence.Append(transform.DOMoveY(peakHeight, timeToFall).SetEase(Ease.OutQuad));
        bounceSequence.Join(transform.DOMoveX(nextXPeak, timeToFall).SetEase(Ease.Linear)); // Di chuyển ngang

        // Rơi xuống
        bounceSequence.Append(transform.DOMoveY(floorY, timeToFall).SetEase(Ease.InQuad));
        bounceSequence.Join(transform.DOMoveX(nextX, timeToFall).SetEase(Ease.Linear)); // Di chuyển ngang


        // Khi hoàn thành lần nảy này, tiếp tục nảy với lực yếu hơn
        bounceSequence.OnComplete(() =>
        {
            transform.DOScaleY(0.5f, 0.1f).SetLoops(2, LoopType.Yoyo);
            SimulateBounce(nextX, floorY, new Vector2(currentVelocity.x *bouncingStrength, -currentVelocity.y * bouncingStrength));
        });
    }
}
