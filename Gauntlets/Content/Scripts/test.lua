local speed = 300.0

function init( entity )
	
end

function update( delta, entity )
	
	entity.Transform.Rotation = entity.Transform.Rotation  + 1 * delta
	
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

	entity.Transform.Translate(translation)
end
