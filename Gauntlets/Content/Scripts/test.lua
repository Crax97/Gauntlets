local speed = 10.0

function init( entity )
	entity.Transform.Position.X = 40
	entity.Transform.Position.Y = 40
end

function update( delta, entity )
	entity.Transform.Rotation = entity.Transform.Rotation  + 1 * delta
end
