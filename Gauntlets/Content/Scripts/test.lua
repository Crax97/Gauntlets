local speed = 100.0
local rotationSpeed = 0.0

local collider
local sprite

local mass = 120.0
local jumpHeight = mass * 2

local velocity = Vector2.Zero
local translation = Vector2.Zero
function init( entity )
	entity.Transform.Position = Vector2.__new(400, 0)
	collider = entity.GetComponent("AABBCollider")
	sprite = entity.GetComponent("AnimatedSprite")
	velocity.Y = 0
end

local canJump = true

function update( delta, entity )
	
	velocity.Y = velocity.Y - mass * delta;

	translation.X = 0
	translation.Y = 0

	--Moving on X axis
	if (Input.KeyIsHeld(Keys.D)) then
		sprite.RenderingEffect = RenderingEffect.None;
		translation.X = (speed * delta)
		sprite.SetAnimation("running");
	elseif (Input.KeyIsHeld(Keys.A)) then
		sprite.RenderingEffect = RenderingEffect.FlipHorizontally;
		translation.X = -(speed * delta)
		sprite.SetAnimation("running");
	elseif not (velocity.Y == 0) then --This means the player is in air
		sprite.SetAnimation("idle");
	end

	if(velocity.Y > 0 ) then
		velocity.Y = velocity.Y - mass * 9.8 * delta
		sprite.SetAnimation("jump")
	end
		
	if(collider.IsColliding()) then
		canJump = true
		if(velocity.Y < 0) then
			velocity.Y = 0
		end

	end
		
	if(Input.KeyHasBeenPressed(Keys.Space) and canJump) then
		canJump = false
		velocity.Y = jumpHeight;
		entity.Transform.Translate(0, -jumpHeight * delta)
	end

	translation = translation - velocity * delta;
	entity.Transform.Translate(translation)
end
