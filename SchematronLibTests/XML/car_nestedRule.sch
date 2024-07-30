<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt3">
	<pattern>
		<rule context="//owner">
			<assert test="firstName and firstName != ''">
				The owner must have a first name and the first name cannot be empty.
			</assert>
			<assert test="lastName and lastName != ''">
				The owner must have a last name and the last name cannot be empty.
			</assert>
		</rule>
	</pattern>
</schema>