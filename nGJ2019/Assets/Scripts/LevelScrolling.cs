using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScrolling : MonoBehaviour
{
  public int updateRate = 1;
  public Transform longcat;
  public Transform background;

  private Vector3 initialBgPos;
  private float[] backgroundSize;

  // Start is called before the first frame update
  void Start()
  {
    ResizeBackground();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void FixedUpdate()
  {
    MoveBackground();
    MoveLongcat();
  }

  private void ResizeBackground()
  {
    var sr = background.GetComponent<SpriteRenderer>();
    if (sr == null) return;

    background.localScale = new Vector3(1, 1, 1);

    var width = sr.sprite.bounds.size.x;
    var height = sr.sprite.bounds.size.y;

    var worldScreenHeight = Camera.main.orthographicSize * 2.0;
    var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

    var finalHeight = (float)(worldScreenHeight / height);
    var finalWidth = (float)(worldScreenWidth / width);

    background.localScale = new Vector3(finalHeight, finalHeight, 1);

    var viewportX = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).x;
    background.position = new Vector3(viewportX * -1, 0, 0);
    initialBgPos = background.position;
  }

  private void MoveLongcat()
  {
    longcat.Translate(new Vector2(0.1f * updateRate, 0));
  }

  private void MoveBackground()
  {
    if (background.position.x > -initialBgPos.x)
      background.Translate(new Vector2(0.1f * updateRate, 0));
    else background.position = initialBgPos;
  }
}
