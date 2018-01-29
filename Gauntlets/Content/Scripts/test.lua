local speed = 10.0
local position = Vector2.__new(400, 300)

function init( entity )
	entity.Transform.Position = position
	Globals.Set("Name", "Test!");
	print ("Init!");
	
end

function update( delta, entity )
	entity.Transform.Rotation = entity.Transform.Rotation  + 1 * delta
	local translation = Vector2.__new(delta * speed, 0)
	entity.Transform.Position = entity.Transform.Position + translation;
	print(entity.Transform.Position.X)
end
