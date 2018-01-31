local speed = 300.0
local rotationSpeed = 0.0

local collider
local sprite

function init( entity )
	
	collider = entity.GetComponent("AABBCollider")
	sprite = entity.GetComponent("AnimatedSprite")
	print(collider)
end

function update( delta, entity )
	
	entity.Transform.Rotation = entity.Transform.Rotation  + rotationSpeed * delta
	
	local translation = Vector2.Zero

	--Moving on X axis
	if (Input.KeyIsHeld(Keys.D)) then
		translation.X = (speed * delta)
	elseif (Input.KeyIsHeld(Keys.A)) then
		translation.X = -(speed * delta)
	end	
	
	--Moving on Y axis
	if (Input.KeyIsHeld(Keys.W)) then
		translation.Y = -(speed * delta)
	elseif (Input.KeyIsHeld(Keys.S)) then
		translation.Y = (speed * delta)
	end

	if(Input.KeyHasBeenPressed(Keys.E)) then
		sprite.SetAnimation("jump")
	end
	
	if(collider.IsColliding()) then
		print("Colliding from lua!")
	end

	entity.Transform.Translate(translation)
end
