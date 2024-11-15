width = ARGV[0].to_i
height = ARGV[1].to_i

if width == 0 || height == 0
    puts("Missing width and height args.")
    exit(1)
end

-1.upto(height).each do |y|
    if y % 2 == 0
        print("    ")
    end

    -1.upto(width).each do |x|
        print("%8s" % "#{x}:#{y}")
    end
    print("\n")
end