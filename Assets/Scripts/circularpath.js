var speed : float = 6.0;
var target : Transform;
private var moveDirection : Vector3 = Vector3.zero;

function Update()
{
        var controller : CharacterController = GetComponent(CharacterController);
        moveDirection = Vector3(Input.GetAxis("Horizontal"), 0,0);
        var worldLookDirection = target.position - transform.position;
        var localLookDirection = transform.InverseTransformDirection(worldLookDirection);
        localLookDirection.y = 0;
        transform.forward = transform.rotation * localLookDirection;
        moveDirection = transform.TransformDirection(moveDirection);
       
        moveDirection *= speed;
    
        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
}