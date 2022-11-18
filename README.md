
# About this project
This project contains tools that enable you to serialize and deserialize complex classes.
A complex class can 
- be recursive, 
- include properties that use special data types that can not easily be serialized,
- include properties that are interfaces or abstract classes
- use binary data (byte array)

The ExtendedXmlSerializer also supports 
- custom xml migration
- custom type serializers

# About nested projects
The XmlTools consists of multiple projects including the main library
- XmlTools
That provides all functionality, classes, helpers, attributes etc.
- Wpf
Includes custom types usually found in WPF projects (currently just a draft)
- Zip
This library uses the ExtendedXmlSerializer to serialize and deserialize classes to a zip file.
Serializing a class to a zip file might be more efficient if you use a lot of resources or multiple resources that might be too much for an xml file to handle (as base 64 string).

# Contribution
Please feel free to contribute to this project.

# Nuget
The libraries created in this project are also avaliable on nuget.org

# Documentation
For detailed documentation, go to the specific ReadMe.md files within the project folders. 
