local speed = 100.0
local rotationSpeed = 0.0

local controller
local sprite
local jumpHeight = 20

local translation = Vector2.Zero
function init( entity )
	entity.Transform.Position = Vector2.__new(400, 0)
	controller = entity.GetComponent("CharacterController")
	controller.Reset()
	sprite = entity.GetComponent("AnimatedSprite")
end

local canJump = true

function update( delta, entity )

	translation.X = 0
	translation.Y = 0

	--Moving on X axis
	if (Input.KeyIsHeld(Keys.D)) then
		sprite.RenderingEffect = RenderingEffect.None;
		translation.X = 1
		--sprite.SetAnimation("running");
	elseif (Input.KeyIsHeld(Keys.A)) then
		sprite.RenderingEffect = RenderingEffect.FlipHorizontally;
		translation.X = -1
		--sprite.SetAnimation("running");
	end

	if(Input.KeyHasBeenPressed(Keys.Space)) then
		controller.Jump(jumpHeight)
	end

	translation = translation * speed * delta;
	controller.Move(translation)




end
