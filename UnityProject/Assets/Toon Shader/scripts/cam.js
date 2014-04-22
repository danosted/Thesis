    var target : Transform;
     
    var targetHeight:float  = 12.0;
    var distance :float = 5.0;
     
    var maxDistance :float = 20.0;
    var minDistance :float = 2.5;
     
    var yawSpeed:float  = 250.0;
    var pitchSpeed :float = 120.0;
     
    var pitchMinLimit:float  = -20.0;
    var pitchMaxLimit :float = 80.0;
     
    var zoomRate:float  = 20.0;
     
    private var yaw :float = 0.0;
    private var pitch :float = 0.0;
	private var _transform :Transform ;
	private var rotation:Quaternion;
	private var position :Vector3;
	 
    function Start () 
 	{
     	Application.targetFrameRate = 200.0;
        var angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
     	_transform = transform;
     	distance = minDistance;

    }
     
    function LateUpdate () 
    {

       yaw += Input.GetAxis( "Horizontal" ) * yawSpeed ;
       
       pitch -= Input.GetAxis( "Vertical" ) * pitchSpeed ;
       pitch = ClampAngle( pitch, pitchMinLimit, pitchMaxLimit );
		
       distance -= ( Input.GetAxis( "Mouse ScrollWheel" ) * Time.deltaTime ) * zoomRate * Mathf.Abs( distance );
       distance = Mathf.Clamp(distance, minDistance, maxDistance);

       rotation = Quaternion.Euler( pitch, yaw, 0.0 );
       position = target.position - ( rotation * Vector3.forward * distance + Vector3( 0.0, -targetHeight, 0.0 ) );

       _transform.rotation = rotation;
       _transform.position = position;
    }
     
    static function ClampAngle (angle : float, min : float, max : float) 
    {
       if (angle < -360.0)
          angle += 360.0;
       if (angle > 360.0)
          angle -= 360.0;
       return Mathf.Clamp (angle, min, max);
    }
     
