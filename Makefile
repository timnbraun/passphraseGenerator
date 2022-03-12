#
# Makefile
#
# Creates a password generator from c# source
#

Configuration = Release

BINDIR = bin/$(Configuration)

# DOTNETDIR=c:/WINDOWS/Microsoft.NET/Framework/v2.0.50727
DOTNETDIR=c:/WINDOWS/Microsoft.NET/Framework/v4.0.30319

CSC = $(DOTNETDIR)/Csc.exe

CSFLAGS = -noconfig -nowarn:1701,1702 -errorreport:prompt -warn:4 -target:exe

CSDEFINES = -define:TRACE 

CSREFS = \
	-reference:$(DOTNETDIR)/System.dll

CSDEBUG = -debug:pdbonly -optimize+ 


SRC = passPhraseGenerator.cs Properties/AssemblyInfo.cs
RESOURCE = Resources/canadian-english
CSRES = -resource:$(RESOURCE),canadian-english,public

bin : $(BINDIR)/passPhraseGenerator.exe

$(BINDIR)/passPhraseGenerator.exe : $(SRC) $(RESOURCE) | $(BINDIR)
	$(CSC) $(CSFLAGS) $(CSDEFINES) $(CSREFS) $(CSDEBUG) $(subst /,\,${CSRES}) -out:$@ $(subst /,\,${SRC})

%.exe : %.cs | ${BINDIR}
	$(CSC) $(CSFLAGS) $(CSDEFINES) $(CSREFS) $(CSDEBUG) -out:$@ $<

$(BINDIR) :
	mkdir -p $@

clean :
	$(RM) -r bin obj
